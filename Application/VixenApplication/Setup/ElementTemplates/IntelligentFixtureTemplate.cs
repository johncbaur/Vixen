//using Orc.Wizard.Example.Views;
using Catel.Collections;
using Catel.IoC;
using Catel.MVVM;
using Orc.Theming;
using Orc.Wizard;
using Orc.Wizard.Example.Wizard;
using Orchestra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Module.OutputFilter;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.Fixture;
using VixenModules.OutputFilter.CoarseFineBreakdown;
using VixenModules.OutputFilter.ColorBreakdown;
using VixenModules.OutputFilter.ColorWheelFilter;
using VixenModules.OutputFilter.DimmingFilter;
using VixenModules.OutputFilter.ShutterFilter;
using VixenModules.OutputFilter.TaggedFilter;
using VixenModules.Property.Color;
using VixenModules.Property.IntelligentFixture;



namespace VixenApplication.Setup.ElementTemplates
{
	public class IntelligentFixtureTemplate : IElementTemplate
	{
		public string TemplateName
		{
			get
			{
				return "Intelligent Fixture";
			}
		}

		private ColorModule CreateColorProperty(ElementNode node)
		{
			node.Properties.Add(ColorDescriptor.ModuleId);

			ColorModule colorProperty = (ColorModule)node.Properties.Single(prop => prop is ColorModule);
			colorProperty.ColorType = ElementColorType.MultipleDiscreteColors;

			return colorProperty;
		}

		private void AddFilterToNode(ElementNode newNode, OutputFilterModuleInstanceBase filter)
		{
			IDataFlowComponent dataFlowComponent = VixenSystem.DataFlow.GetComponent(newNode.Element.Id);

			DataFlowComponentReference dataFlowComponentReference =
				new DataFlowComponentReference(dataFlowComponent, 0);

			VixenSystem.DataFlow.SetComponentSource(filter, dataFlowComponentReference);
			VixenSystem.Filters.AddFilter(filter);
		}

		private void ProcessColorWheelFunction(bool isLampFixture, FixtureFunction function, ElementNode newNode, string fixtureName)
		{
			// Create the color wheel filter
			ColorWheelFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(ColorWheelFilterDescriptor.ModuleId) as
					ColorWheelFilterModule;

			// Give the filter the color wheel data
			filter.ColorWheelData = function.ColorWheelData;
			
			// Configure the filter on whether to convert RGB Color into color wheel index commands
			// Generally for LED fixtures this will be configured to false
			// Generally for lamp fixtures this will configure to true
			filter.ConvertRGBIntoIndexCommands = isLampFixture;
			filter.Tag = function.Name;

			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the filter to the display element node
			AddFilterToNode(newNode, filter);

			// Create a color property
			ColorModule colorProperty = CreateColorProperty(newNode);

			// If the fixture is lamp fixture then...
			if (isLampFixture)
			{
				// Configure the color property to use discrete colors
				colorProperty.ColorType = ElementColorType.MultipleDiscreteColors;

				// Create a new color set to hold the discrete colors
				ColorSet colorSet = new ColorSet();

				// Loop over the color wheel data
				foreach (FixtureColorWheel colorWheelData in function.ColorWheelData)
				{
					// If the color is a unique color then...
					if (!colorWheelData.HalfStep &&
					    !colorWheelData.UseCurve)
					{
						// Add the color to the color set
						colorSet.Add(Color.FromArgb(colorWheelData.Color1.ToArgb()));
					}
				}

				// Retrieve the static color data associated with the color property
				ColorStaticData staticData = (ColorStaticData)colorProperty.StaticModuleData;

				// Add / update the color set
				staticData.SetColorSet(fixtureName, colorSet);

				// Name the color set after the 
				colorProperty.ColorSetName = fixtureName;
			}
			else
			{
				//TODO: I think this code should be elsewhere, I don't think we are guaranteed that all LED fixtures
				//TODO: will have colorwheels!
				// Configure full color (RGB Support)
				colorProperty.ColorType = ElementColorType.FullColor;
			}
		}

		private ColorBreakdownItem CreateColorBreakdownItem(Color color, string colorName)
		{
			ColorBreakdownItem cbi = new ColorBreakdownItem();
			cbi.Color = color;
			cbi.Name = colorName;

			return cbi;
		}

		private void ProcessRGBWFunction(
			ref bool foundRGBWChannel,
			ref int rgbwChannelCount,
			ElementNode newNode)
		{
			// If this is the first RGBW channel encountered then...
			if (!foundRGBWChannel)
			{
				// Remember that we encountered an RGBW channel
				foundRGBWChannel = true;

				// Keep track of the number of RGBW channels encountered
				// Expect to see 4 consecutive channels
				rgbwChannelCount++;

				// Create the color breakdown module
				ColorBreakdownModule filter =
					ApplicationServices.Get<IOutputFilterModuleInstance>(ColorBreakdownDescriptor.ModuleId)
						as
						ColorBreakdownModule;

				// Create the list of color breakdown items
				List<ColorBreakdownItem> newBreakdownItems = new List<ColorBreakdownItem>();

				// Create the color break down items for RGBW
				newBreakdownItems.Add(CreateColorBreakdownItem(Color.Red, "Red"));
				newBreakdownItems.Add(CreateColorBreakdownItem(Color.Lime, "Green"));
				newBreakdownItems.Add(CreateColorBreakdownItem(Color.Blue, "Blue"));
				newBreakdownItems.Add(CreateColorBreakdownItem(Color.White, "White"));

				// Associate the color breakdown items with the filter
				filter.BreakdownItems = newBreakdownItems;

				// Indicate the color mix to create the desired color
				filter.MixColors = true;

				// Add the breakdown module to the node
				AddFilterToNode(newNode, filter);
			}
			// Otherwise this is the 2nd, 3rd or 4th RGBW channel
			else
			{
				// Increment the number of RGBW channels encountered
				rgbwChannelCount++;

				// If the 4th RGBW channel was processed then...
				if (rgbwChannelCount == 4)
				{
					// Reset the RGBW flags
					rgbwChannelCount = 0;
					foundRGBWChannel = false;
				}
			}
		}

		private void ProcessDimmerFunction(ElementNode newNode, FixtureFunction function, bool isLampFixture)
		{
			// Create the dimming filter
			DimmingFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(DimmingFilterDescriptor.ModuleId) as
					DimmingFilterModule;

			// Assign the filter the function name as the tag
			filter.Tag = function.Name;

			// Configure whether the filter should convert RGB intents into dimming intents
			// Generally this only applies to lamp fixtures
			filter.ConvertRGBIntoDimmingIntents = isLampFixture;

			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the dimming filter to the node
			AddFilterToNode(newNode, filter);
		}

		private void ProcessShutterFunction(ElementNode newNode, FixtureFunction function)
		{
			// Create the shutter filter
			ShutterFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(ShutterFilterDescriptor.ModuleId) as
					ShutterFilterModule;

			byte openShutter = 0;

			// Loop over the enumerations associated with the function
			foreach (FixtureIndex fixtureIndex in function.IndexData)
			{
				// If an open shutter index is found then...
				if (fixtureIndex.IndexType == FixtureIndexType.ShutterOpen)
				{
					// Save off the numeric value of the Open Shutter
					openShutter = (byte)fixtureIndex.StartValue;
					break;
				}
			}

			// Assign the function name as the filter tag
			filter.Tag = function.Name;
			
			// Configure whether to generate shutter intents when a color intent is encountered
			// This setting applies to both LED and Lamp fixtures
			filter.ConvertRGBIntoShutterIntents = true;
			
			// Assign the open shutter index value
			filter.OpenShutterIndexValue = openShutter;
			
			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the shutter filter to the node
			AddFilterToNode(newNode, filter);
		}

		private void ProcessTaggedFunction(ElementNode newNode, FixtureFunction function)
		{
			// Create the tagged filter
			TaggedFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(TaggedFilterDescriptor.ModuleId) as
					TaggedFilterModule;

			// Assign the function name as the tag
			filter.Tag = function.Name;

			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the tagged filter to the node
			AddFilterToNode(newNode, filter);
		}

		private void AddCourseFineBreakdown(ElementNode newNode)
		{
			// Create course / fine breakdown module
			CoarseFineBreakdownModule courseFineBreakdown =
				ApplicationServices.Get<IOutputFilterModuleInstance>(CoarseFineBreakdownDescriptor.ModuleId) as
					CoarseFineBreakdownModule;

			// Find the leafs of the node
			IList<IDataFlowComponentReference> nodes = FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(newNode.Element.Id)).ToList();

			// Add the breakdown module to the last node
			VixenSystem.DataFlow.SetComponentSource(courseFineBreakdown, nodes[nodes.Count - 1]);
			VixenSystem.Filters.AddFilter(courseFineBreakdown);
		}
		
		private Task OnShowWizardxEecuteAsync(Type wizardType)
		{
			//this.ApplyTheme();

			var _typeFactory = this.GetTypeFactory();

			var wizard = _typeFactory.CreateInstance(wizardType) as IExampleWizard;

			wizard.ShowInTaskbarWrapper = false;
			wizard.ShowHelpWrapper = false;
			wizard.AllowQuickNavigationWrapper = true;
			wizard.HandleNavigationStatesWrapper = true;
			wizard.CacheViewsWrapper = true;

			bool UseFastForwardNavigationController = true;
			bool ShowSummaryPage = true;
			bool MarkAllPagesAsVisited = false;
			
			if (UseFastForwardNavigationController)
			{
				wizard.NavigationControllerWrapper = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<FastForwardNavigationController>(wizard);
			}

			if (!ShowSummaryPage)
			{
				var lastPage = wizard.Pages.Last();
				wizard.RemovePage(lastPage);
			}

			if (MarkAllPagesAsVisited)
			{
				wizard.Pages.ForEach(x => x.IsVisited = true);
			}

			IDependencyResolver dependencyResolver = this.GetDependencyResolver();
			IWizardService wizardService = (IWizardService)dependencyResolver.Resolve(typeof(IWizardService));

			return wizardService.ShowWizardAsync(wizard);			
		}

		public TaskCommand<Type> ShowWizard { get; set; }


		public IEnumerable<ElementNode> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
		{
			// WORKS BUT THREAD CONTINUES AND DISPLAYS FIXTURE PROPERTY EDITOR			
			{
				Orc.Theming.ThemeManager.Current.SynchronizeTheme();

				IDependencyResolver dependencyResolver = this.GetDependencyResolver();
				IBaseColorSchemeService baseColorService = (IBaseColorSchemeService)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));

				baseColorService.SetBaseColorScheme("Dark");

				IAccentColorService accentColorServer = (IAccentColorService)dependencyResolver.Resolve(typeof(IAccentColorService));
				accentColorServer.SetAccentColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("DodgerBlue"));

				ShowWizard = new TaskCommand<Type>(OnShowWizardxEecuteAsync);
				ShowWizard.Execute(typeof(ExampleSideNavigationWizard));
			}
			// END WORKS BUT THREAD CONTINUES AND DISPLAYS FIXTURE PROPERTY EDITOR

			
			// WORKS WITH DEVELOPER DIALOG
			//Orc.Wizard.Example.Views.MainView mainView = new Orc.Wizard.Example.Views.MainView();
			//mainView.ShowDialog();
			// END WORKS WITH DEVELOPER DIALOG

			bool isLampFixture = true;
			
			// Create the new display element node
			ElementNode newNode = ElementNodeService.Instance.CreateSingle(null, "MH1");

			// Add the intelligent fixture property to the new node
			newNode.Properties.Add(ZoomDescriptor._typeId);

			// Retrieve the intelligent 
			IntelligentFixtureModule intelligentFixtureProperty = 
				(IntelligentFixtureModule)newNode.Properties.Single(prop => prop is IntelligentFixtureModule);

			// Display the intelligent fixture setup dialog
			intelligentFixtureProperty.Setup();

			// Retrieve the fixture specification
			FixtureSpecification fixture = intelligentFixtureProperty.GetFixtureSpecification();

			// Local variable to keep track of the last function processed
			string lastFunction = string.Empty;
			
			// Local variables to keep track of if we have encountered an RGBW channel
			bool foundRGBWChannel = false;
			int rgbwChannelCount = 0;

			// Loop over the channels on the fixture
			int channelIndex = 0;
			foreach (FixtureChannel channel in fixture.ChannelDefinitions)
			{
				// Declare variables for the next 
				FixtureChannel nextChannel = null;
				FixtureFunction nextChannelFunction = null;

				// If there are additional channels in the definitions then...
				if (channelIndex + 1 < fixture.ChannelDefinitions.Count)
				{
					// Retrieve the next channel
					nextChannel = fixture.ChannelDefinitions[channelIndex + 1];

					// Retrieve the next channel's function
					nextChannelFunction = fixture.FunctionDefinitions.Single(fn => fn.Name == nextChannel.Function);
				}

				// Retrieve the function associated with the current channel
				FixtureFunction function = fixture.FunctionDefinitions.Single(fn => fn.Name == channel.Function);
				
				// If the channel's function is different from the last function processed then...
				if (channel.Function != lastFunction)
				{
					// Update the last function processed
					lastFunction = channel.Function;

					// If the function is a Color Wheel and
					// the function has a name then...
					if (function.FunctionType == FixtureFunctionType.ColorWheel &&
					    !string.IsNullOrEmpty(function.Name))
					{
						ProcessColorWheelFunction(isLampFixture, function, newNode, fixture.Name);
					}
					// If the function is RGBW and
					// the function has a name then...
					else if (function.FunctionType == FixtureFunctionType.RGBWColor &&
					    !string.IsNullOrEmpty(function.Name))
					{
						ProcessRGBWFunction(ref foundRGBWChannel, ref rgbwChannelCount, newNode);
					}
					// If the function is a dimming function and
					// the function has a name then...
					else if (function.FunctionIdentity == FunctionIdentity.Dim &&
					         !string.IsNullOrEmpty(function.Name))
					{
						ProcessDimmerFunction(newNode, function, isLampFixture);
					}
					// If the function is a shutter function and
					// the function has a name then...
					else if (function.FunctionIdentity == FunctionIdentity.Shutter &&
							  !string.IsNullOrEmpty(function.Name))
					{
						ProcessShutterFunction(newNode, function);
					}
					// Otherwise this is a generic tagged function
					else
					{
						ProcessTaggedFunction(newNode, function);
					}

					// If the next two channels are range functions with the same function name then...
					if (function.FunctionType == FixtureFunctionType.Range &&
					    nextChannelFunction != null &&
					    nextChannelFunction.FunctionType == FixtureFunctionType.Range &&
					    function.Name == nextChannelFunction.Name)
					{
						// Add a course / fine breakdown module
						AddCourseFineBreakdown(newNode);
					}
				}

				// Increment the channel counter
				channelIndex++;
			}

			// Return the new node as an array
			return new[] { newNode };
		}

		private IEnumerable<IDataFlowComponentReference> FindLeafOutputsOrBreakdownFilters(IDataFlowComponent component)
		{
			if (component == null)
			{
				yield break;
			}

			if (component is ColorBreakdownModule)
			{
				yield return new DataFlowComponentReference(component, -1);
				// this is a bit iffy -- -1 as a component output index -- but hey.
			}

			if (component.Outputs == null || component.OutputDataType == DataFlowType.None)
			{
				yield break;
			}

			for (int i = 0; i < component.Outputs.Length; i++)
			{
				IEnumerable<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(component, i);

				if (!children.Any())
				{
					yield return new DataFlowComponentReference(component, i);
				}
				else
				{
					foreach (IDataFlowComponent child in children)
					{
						foreach (IDataFlowComponentReference result in FindLeafOutputsOrBreakdownFilters(child))
						{
							yield return result;
						}
					}
				}
			}
		}

		public bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null)
		{
			return true;
		}

		public bool ConfigureColor => false;

		public bool ConfigureDimming => false;
	}
}
