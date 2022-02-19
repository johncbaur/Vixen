using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Fixture;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;
using VixenModules.Property.IntelligentFixture;
using ZedGraph;

namespace VixenModules.Effect.Fixture
{
	public class Fixture : BaseEffect
	{
		#region Constructor 

		public Fixture()
		{
			// Create the collection of waves
			_functionItemCollection = new FixtureFunctionCollection();
			//_fixtureFunctions = new List<App.Fixture.FixtureFunction>();
			_fixtureSpecifications = new List<FixtureSpecification>();
		}

		#endregion

		private FixtureData _data;
		private EffectIntents _effectIntents;
				
		
		#region Public Properties

		public override IModuleDataModel ModuleData
		{
			get
			{

				UpdateModelData();
				return _data;				
			}
			set
			{
				_data = (FixtureData)value;
				//if (_fixtureFunctions.Count == 0 )
				//{
				//	GetDeviceFunctions();
				//}				
				UpdateFunctionViewModel(_data);
			}
		}

		private void UpdateModelData()
		{
			_data.FunctionData.Clear();

			foreach(IFixtureFunction functionViewModel in Functions)
			{
				FixtureFunctionData functionModel = new FixtureFunctionData();
				functionModel.FunctionName = functionViewModel.FunctionName;
				functionModel.Range = new Curve(functionViewModel.Range);
				functionModel.FunctionIdentity = functionViewModel.FunctionIdentity;
				functionModel.IndexValue = functionViewModel.IndexValue;
				functionModel.ColorIndexValue = functionViewModel.ColorIndexValue;
				functionModel.FunctionType = functionViewModel.FunctionType;
				functionModel.Enable = functionViewModel.Enable;

				_data.FunctionData.Add(functionModel);
			}
		}

		[ProviderCategory(@"Preview", 1)]
		[ProviderDisplayName(@"Show Legend")]
		[ProviderDescription(@"Show Legend")]
		[PropertyOrder(1)]
		public bool ShowLegend
		{
			get
			{
				return _data.ShowLegend;
			}
			set
			{
				_data.ShowLegend = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}


		private FixtureFunctionCollection _functionItemCollection;

		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Functions")]
		[ProviderDescription(@"Functions")]
		[PropertyOrder(2)]
		public FixtureFunctionCollection Functions
		{
			get
			{
				return _functionItemCollection;
			}
			set
			{
				_functionItemCollection = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/SetZoom/"; }
		}

		#endregion
		
		/// <summary>
		/// Updates the visibility of Pan / Tilt attributes.
		/// </summary>
		private void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{				
				//{ nameof(Zoom), _canZoom},
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		
		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_effectIntents = new EffectIntents();

			foreach (FixtureFunction function in Functions)
			{
				if (function.Enable)
				{
					switch (function.FunctionType)
					{
						case FixtureFunctionType.Range:
							RenderCurve(function.Range, function.FunctionIdentity, GetFunction(function.FunctionName), cancellationToken);
							break;
						case FixtureFunctionType.Indexed:
							RenderIndexed(function.IndexValue, function.Range, GetFunction(function.FunctionName), cancellationToken);
							break;
						case FixtureFunctionType.ColorWheel:
							RenderColorWheelIndex(function.ColorIndexValue, function.Range, GetFunction(function.FunctionName), cancellationToken);
							break;
						case FixtureFunctionType.RGBWColor:
							RenderRGBW(function.Color, function.Range, GetFunction(function.FunctionName), cancellationToken);
							break;
					}
				}
			}

			UpdateModelData();
		}

		/// <inheritdoc />
		protected override void TargetNodesChanged()
		{
			UpdateFunctionCapabilities();
			UpdateAttributes();
		}

		protected override EffectIntents _Render()
		{
			return _effectIntents;
		}

		protected int GetNumberFrames()
		{
			if (TimeSpan == TimeSpan.Zero)
			{
				return 0;
			}
			return (int)(TimeSpan.TotalMilliseconds / FrameTime);
		}

		/*
		private void RenderCurve(Curve c, RangeType rangeType, CancellationTokenSource cancellationToken = null)
		{

			var nodes = GetRenderNodesForType(rangeType);
			if (!nodes.Any()) return;
			c.Points.Sort();

			HashSet<double> points = new HashSet<double> { 0.0 };
			foreach (PointPair point in c.Points)
			{
				points.Add(point.X);
			}
			points.Add(100.0);
			var pointList = points.ToList();
			TimeSpan startTime = TimeSpan.Zero;
			for (int i = 1; i < points.Count; i++)
			{
				ZoomValue startValue = new ZoomValue(c.GetValue(pointList[i - 1]) / 100d);
				ZoomValue endValue = new  ZoomValue(c.GetValue(pointList[i]) / 100d);

				TimeSpan timeSpan = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds * ((pointList[i] - pointList[i - 1])/100));
				ZoomIntent intent = new ZoomIntent(startValue, endValue, timeSpan);
				foreach (IElementNode node in nodes)
				{
					if (cancellationToken != null && cancellationToken.IsCancellationRequested)
						return;

					if (node != null)
					{
						_effectIntents.AddIntentForElement(node.Element.Id, intent, startTime);
					}
				}

				startTime = startTime + timeSpan;
			}
		}
		*/

		protected double GetEffectTimeIntervalPosition(int frame)
		{
			double position;
			if (TimeSpan == TimeSpan.Zero)
			{
				position = 0;
			}
			else
			{
				position = (frame * FrameTime) / TimeSpan.TotalMilliseconds;
			}
			return position;
		}

		#region Intent Methods

		private void RenderColorWheelIndex(string colorWheelValue, Curve c, App.Fixture.FixtureFunction function, CancellationTokenSource cancellationToken = null)
		{
			if (colorWheelValue != null)
			{
				IEnumerable<IElementNode> nodes = GetRenderNodesForFunctionType(function.Name);

				// Short circuit if there aren't any nodes?
				if (!nodes.Any())
				{
					return;
				}

				List<FixtureColorWheel> fixtureColorWheel = function.ColorWheelData;

				TimeSpan startTime = TimeSpan.Zero;

				int numberOfFrames = GetNumberFrames();
				var frameTs = new TimeSpan(0, 0, 0, 0, FrameTime);

				for (int frameNum = 0; frameNum < numberOfFrames; frameNum++)
				{
					foreach (IElementNode node in nodes)
					{
						FixtureColorWheel selectedColorWheelItem = fixtureColorWheel.Single(item => item.Name == colorWheelValue);

						int indxValue = selectedColorWheelItem.StartValue;

						Named8BitCommand namedCommand = new Named8BitCommand(indxValue);
						namedCommand.Tag = function.Name;
						namedCommand.IndexType = (int)FixtureIndexType.ColorWheel;
						//namedCommand.IndexType = (int)FixtureIndexType.ColorWheel;
						//namedCommand.Legend = function.Legend;
						CommandValue commandValue = new CommandValue(namedCommand);
						CommandIntent commandIntent = new CommandIntent(commandValue, TimeSpan);

						_effectIntents.AddIntentForElement(node.Element.Id, commandIntent, startTime);
						startTime = startTime + frameTs;
					}
				}
			}
		}

		private void RenderIndexed(string indexValue, Curve c, App.Fixture.FixtureFunction function, CancellationTokenSource cancellationToken = null)
		{
			IEnumerable<IElementNode> nodes = GetRenderNodesForFunctionType(function.Name);

			// Short circuit if there aren't any nodes?
			if (!nodes.Any())
			{
				return;
			}

			FixtureIndex fixtureIndex = function.IndexData.Single(item => item.Name == indexValue);

			TimeSpan startTime = TimeSpan.Zero;

			int numberOfFrames = GetNumberFrames();
			var frameTs = new TimeSpan(0, 0, 0, 0, FrameTime);

			for (int frameNum = 0; frameNum < numberOfFrames; frameNum++)
			{
				foreach (IElementNode node in nodes)
				{
					int indxValue = fixtureIndex.StartValue;

					if (fixtureIndex.UseCurve)
					{						
						double intervalPos = GetEffectTimeIntervalPosition(frameNum);
						indxValue = (int)Math.Round(ScaleCurveToValue(c.GetValue(intervalPos), fixtureIndex.EndValue, fixtureIndex.StartValue));			
					}
					
					Named8BitCommand namedCommand = new Named8BitCommand(indxValue);
					namedCommand.Tag = function.Name;
					namedCommand.IndexType = (int)fixtureIndex.IndexType;

					if (_data.ShowLegend)
					{
						namedCommand.Legend = function.Legend;
					}
					CommandValue commandValue = new CommandValue(namedCommand);
					CommandIntent commandIntent = new CommandIntent(commandValue, TimeSpan);
					
					_effectIntents.AddIntentForElement(node.Element.Id, commandIntent, startTime);
					startTime = startTime + frameTs;
				}
			}
		}

		private void RenderCurve(Curve c, FunctionIdentity functionIdentity, App.Fixture.FixtureFunction function, CancellationTokenSource cancellationToken = null)
		{
			IEnumerable<IElementNode> nodes = GetRenderNodesForFunctionType(function.Name);

			if (!nodes.Any()) return;
			c.Points.Sort();

			HashSet<double> points = new HashSet<double> { 0.0 };
			foreach (PointPair point in c.Points)
			{
				points.Add(point.X);
			}
			points.Add(100.0);
			var pointList = points.ToList();
			TimeSpan startTime = TimeSpan.Zero;
			for (int i = 1; i < points.Count; i++)
			{
				//RangeValue startValue = new RangeValue((int)functionIdentity, function.Name,  c.GetValue(pointList[i - 1]) / 100d);
				//RangeValue endValue = new RangeValue((int)functionIdentity, function.Name, c.GetValue(pointList[i]) / 100d);

				RangeValue<FunctionIdentity> startValue = new RangeValue<FunctionIdentity> (functionIdentity, function.Name, c.GetValue(pointList[i - 1]) / 100d);
				RangeValue<FunctionIdentity> endValue = new RangeValue<FunctionIdentity>(functionIdentity, function.Name, c.GetValue(pointList[i]) / 100d);

				TimeSpan timeSpan = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds * ((pointList[i] - pointList[i - 1]) / 100));
				//RangeIntent intent = new RangeIntent(startValue, endValue, timeSpan);
				RangeIntent intent = new RangeIntent(startValue, endValue, timeSpan);

				foreach (IElementNode node in nodes)
				{
					if (cancellationToken != null && cancellationToken.IsCancellationRequested)
						return;

					if (node != null)
					{
						_effectIntents.AddIntentForElement(node.Element.Id, intent, startTime);
					}
				}

				startTime = startTime + timeSpan;
			}
		}

		#endregion

		List<FixtureSpecification> _fixtureSpecifications;
		//List<App.Fixture.FixtureFunction> _fixtureFunctions;

		private IEnumerable<string> GetIndexedItems(string functionName)
		{
			App.Fixture.FixtureFunction function = _data.FixtureFunctions.Single(fn => fn.Name == functionName);
			return function.IndexData.Select(item => item.Name);
		}

		private App.Fixture.FixtureFunction GetFunction(string functionName)
		{
			return _data.FixtureFunctions.Single(fn => fn.Name == functionName);
		}

		private void UpdateFunctionCapabilities()
		{
			// Clear fixture function data
			//_data.FunctionData.Clear();

			// Clear the collection of supported functions
			//_data.FixtureFunctions.Clear();

			List<App.Fixture.FixtureFunction> fixtureFunctions = GetDeviceFunctions();

			List<App.Fixture.FixtureFunction> functionsToRemove = new List<App.Fixture.FixtureFunction>();
			foreach (App.Fixture.FixtureFunction function in _data.FixtureFunctions)
			{
				if (!fixtureFunctions.Any(fn => fn.Name == function.Name))
				{
					functionsToRemove.Add(function);
				}
			}

			// Remove any functions that are no longer supported
			foreach(App.Fixture.FixtureFunction function in functionsToRemove)
			{
				_data.FixtureFunctions.Remove(function);
			}

			// Add any new functions
			foreach (App.Fixture.FixtureFunction function in fixtureFunctions)
			{
				if (!_data.FixtureFunctions.Any(fn => fn.Name == function.Name))
				{
					_data.FixtureFunctions.Add(function);
				}
			}

			foreach (App.Fixture.FixtureFunction func in _data.FixtureFunctions)
			{
				// Only include the function in the effect if it was configured to be included
				//JCB if (func.IncludeInEffect)
				{
					if (!_data.FunctionData.Any(item => item.FunctionName == func.Name))
					{
						// Look at the type of function being processed
						switch (func.FunctionType)
						{
							case FixtureFunctionType.Range:
								{
									FixtureFunctionData item = new FixtureFunctionData();
									item.FunctionIdentity = func.FunctionIdentity;
									item.FunctionName = func.Name;
									item.FunctionType = func.FunctionType;
									_data.FunctionData.Add(item);
								}
								break;
							case FixtureFunctionType.Indexed:
								{
									FixtureFunctionData item = new FixtureFunctionData();
									item.FunctionIdentity = func.FunctionIdentity;
									item.FunctionName = func.Name;
									item.FunctionType = func.FunctionType;
									item.IndexValue = GetIndexedItems(func.Name).FirstOrDefault();
									_data.FunctionData.Add(item);
								}
								break;
							case FixtureFunctionType.ColorWheel:
								{
									//TODO: Need to think about Color Wheels!
									FixtureFunctionData item = new FixtureFunctionData();
									item.FunctionIdentity = func.FunctionIdentity;
									item.FunctionName = func.Name;
									item.FunctionType = func.FunctionType;
									item.IndexValue = GetIndexedItems(func.Name).FirstOrDefault();
									_data.FunctionData.Add(item);
								}
								break;
							case FixtureFunctionType.RGBColor:
							case FixtureFunctionType.RGBWColor:
								{
									FixtureFunctionData item = new FixtureFunctionData();
									item.FunctionIdentity = func.FunctionIdentity;
									item.FunctionName = func.Name;
									item.FunctionType = func.FunctionType;
									_data.FunctionData.Add(item);
								}
								break;
						}
						
						//_data.FunctionData.Add(item);
					}
				}
				UpdateFunctionViewModel(_data);
			}
			
			/*
			// Get the target node for the effect
			IElementNode node = TargetNodes.FirstOrDefault();

			// Make sure there is a target node
			if (node != null)
			{
				// Create a collection to hold the functions the fixture supports
				List<string> functions = new List<string>();

				// If the effect was applied to a group we need to process the leaf nodes
				foreach (IElementNode leafNode in node.GetLeafEnumerator())
				{
					// Attempt to get the Intelligent Fixture property
					IntelligentFixtureModule fixtureProperty = (IntelligentFixtureModule)leafNode.Properties.FirstOrDefault(item => item is IntelligentFixtureModule);

					// If a fixture property was found then...
					if (fixtureProperty != null)
					{
						// Get the fixture associated with the leaf node
						FixtureSpecification fixtureSpecification = fixtureProperty.GetFixtureSpecification();

						// Add the fixture to the collection of target fixtures
						_fixtureSpecifications.Add(fixtureSpecification);

						// Loop over the channels of the fixture
						foreach (FixtureChannel channel in fixtureSpecification.ChannelDefinitions)
						{
							// Get the function associated with the channel
							App.Fixture.FixtureFunction func = fixtureSpecification.FunctionDefinitions.Single(function => function.Name == channel.Function);

							// Only include the function in the effect if it was configured to be included
							if (func.IncludeInEffect)
							{
								// If the function has not already been processed then...
								if (!functions.Contains(func.Name))
								{
									// Add the function to the collection of supported functions
									_data.FixtureFunctions.Add(func);

									// Look at the type of function being processed
									switch (func.FunctionIdentity)
									{
										case FixtureFunctionType.Range:
											{
												FixtureFunctionData item = new FixtureFunctionData();
												item.FunctionIdentity = GetFunctionIdentity(func.Name);
												item.Tag = func.Name;
												item.FunctionIdentity = func.FunctionIdentity;
												_data.FunctionData.Add(item);
											}
											break;
										case FixtureFunctionType.Indexed:
											{
												FixtureFunctionData item = new FixtureFunctionData();
												item.FunctionIdentity = GetFunctionIdentity(func.Name);
												item.Tag = func.Name;
												item.FunctionIdentity = func.FunctionIdentity;
												item.IndexValue = GetIndexedItems(func.Name).FirstOrDefault();
												_data.FunctionData.Add(item);
											}
											break;
										case FixtureFunctionType.ColorWheel:
											{
												//TODO: Need to think about Color Wheels!
												FixtureFunctionData item = new FixtureFunctionData();
												item.FunctionIdentity = GetFunctionIdentity(func.Name);
												item.Tag = func.Name;
												item.FunctionIdentity = func.FunctionIdentity;
												item.IndexValue = GetIndexedItems(func.Name).FirstOrDefault();
												_data.FunctionData.Add(item);
											}
											break;
										case FixtureFunctionType.RGBColor:
										case FixtureFunctionType.RGBWColor:
											{
												FixtureFunctionData item = new FixtureFunctionData();
												item.FunctionIdentity = GetFunctionIdentity(func.Name);
												item.Tag = func.Name;
												item.FunctionIdentity = func.FunctionIdentity;
												_data.FunctionData.Add(item);
											}
											break;
									}

									functions.Add(func.Name);
								}
							}
						}
					}
				}
				UpdateFunctionViewModel(_data);
			}
			*/
		}

		
		private List<App.Fixture.FixtureFunction> GetDeviceFunctions()
		{			
			// Clear the collection of supported functions
			//_data.FixtureFunctions.Clear();

			List<App.Fixture.FixtureFunction> fixtureFunctions = new List<App.Fixture.FixtureFunction>();

			// Get the target node for the effect
			IElementNode node = TargetNodes.FirstOrDefault();

			// Make sure there is a target node
			if (node != null)
			{
				// Create a collection to hold the functions the fixture supports
				List<string> functions = new List<string>();

				// If the effect was applied to a group we need to process the leaf nodes
				foreach (IElementNode leafNode in node.GetLeafEnumerator())
				{
					// Attempt to get the Intelligent Fixture property
					IntelligentFixtureModule fixtureProperty = (IntelligentFixtureModule)leafNode.Properties.FirstOrDefault(item => item is IntelligentFixtureModule);

					// If a fixture property was found then...
					if (fixtureProperty != null)
					{
						// Get the fixture associated with the leaf node
						FixtureSpecification fixtureSpecification = fixtureProperty.GetFixtureSpecification();
						
						// Loop over the channels of the fixture
						foreach (FixtureChannel channel in fixtureSpecification.ChannelDefinitions)
						{
							// Get the function associated with the channel
							App.Fixture.FixtureFunction func = fixtureSpecification.FunctionDefinitions.Single(function => function.Name == channel.Function);

							// Only include the function in the effect if it was configured to be included
							//JCB if (func.IncludeInEffect)
							{
								// If the function has not already been processed then...
								if (!functions.Contains(func.Name))
								{
									// Add the function to the collection of supported functions
									fixtureFunctions.Add(func);
								
									functions.Add(func.Name);
								}
							}
						}
					}
				}			
			}

			return fixtureFunctions;
		}
		
		/*
		private FunctionIdentity GetFunctionIdentity(string functionName)
		{
			// Default the function identity to Custom
			FunctionIdentity identity = FunctionIdentity.Custom;

			if (functionName == "Pan")
			{
				identity = FunctionIdentity.Pan;
			}
			else if (functionName == "Tilt")
			{
				identity = FunctionIdentity.Tilt;
			}
			else if (functionName == "Zoom")
			{
				identity = FunctionIdentity.Zoom;
			}
			else if (functionName == "Dim")
			{
				identity = FunctionIdentity.Dim;
			}

			return identity;
		}
		*/

		
		private IEnumerable<IElementNode> GetRenderNodesForFunctionType(string functionName)
		{
			IElementNode node = TargetNodes.FirstOrDefault();
			
			if (node == null)
			{
				return Enumerable.Empty<IElementNode>();
			}

			IList<IElementNode> renderNodes = new List<IElementNode>();

			foreach (IElementNode leafNode in node.GetLeafEnumerator())
			{
				// Attempt to get the Intelligent Fixture property
				IntelligentFixtureModule fixtureProperty = (IntelligentFixtureModule)leafNode.Properties.FirstOrDefault(item => item is IntelligentFixtureModule);

				// If a fixture property was found then...
				if (fixtureProperty != null)
				{
					if (fixtureProperty.GetFixtureSpecification().SupportsFunction(functionName))
					{
						renderNodes.Add(leafNode);
					}
				}
			}

			return renderNodes;							
		}



		#region Overrides of BaseEffect

		/// <inheritdoc />
		protected override EffectTypeModuleData EffectModuleData 
		{ 
			get
			{
				UpdateModelData();
				return _data;
			}
		}

		public override bool ForceGenerateVisualRepresentation { get { return true; } }

		public override void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle)
		{			
			var rect = new Rectangle(clipRectangle.X, clipRectangle.Y, clipRectangle.Width, clipRectangle.Height-4);
			var textRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
			var f = Vixen.Common.Graphics.GetAdjustedFont(g, "Zoom", textRect, SystemFonts.MessageBoxFont.Name, 24, SystemFonts.MessageBoxFont);

			//if (_canZoom)
			{
				var zoomColor = Color.Green;
				var panCurve = Functions[0].Range.GenerateGenericCurveImage(new Size(rect.Width, rect.Height),false, false, false, zoomColor);
				g.DrawImage(panCurve, rect.X, rect.Y+2);
				
				g.DrawString("Zoom", f, new SolidBrush(zoomColor), rect.X, 1);
			}						
		}

		#endregion

		/// <summary>
		/// Converts from the serialized wave data to the model wave data.
		/// </summary>		
		private void UpdateFunctionViewModel(FixtureData fixtureData)
		{
			// Clear the view model range collection
			Functions.Clear();

			// Loop over the ranges in the serialized effect data
			foreach (FixtureFunctionData serializedWaveform in fixtureData.FunctionData)
			{
				// Create a new range in the model
				IFixtureFunction range = new FixtureFunction(_data.FixtureFunctions);

				// Transfer the properties from the serialized effect data to the funtion model
				range.Range = new Curve(serializedWaveform.Range);
				range.FunctionIdentity = serializedWaveform.FunctionIdentity;
				range.FunctionName = serializedWaveform.FunctionName;
				range.FunctionType = serializedWaveform.FunctionType;				
				range.IndexValue = serializedWaveform.IndexValue;
				range.ColorIndexValue = serializedWaveform.ColorIndexValue;
				range.Enable = serializedWaveform.Enable;
				
				// Add the range the effect's collection
				Functions.Add(range);
			}			
		}

		private void RenderRGBW(ColorGradient colorGradient, Curve intensity, App.Fixture.FixtureFunction function, CancellationTokenSource cancellationToken = null)
		{
			IEnumerable<IElementNode> nodes = GetRenderNodesForFunctionType(function.Name);

			// Short circuit if there aren't any nodes?
			if (!nodes.Any())
			{
				return;
			}
			
			//TimeSpan startTime = TimeSpan.Zero;

			int numberOfFrames = GetNumberFrames();
			//var frameTs = new TimeSpan(0, 0, 0, 0, FrameTime);

			for (int frameNum = 0; frameNum < numberOfFrames; frameNum++)
			{
				double intervalPos = GetEffectTimeIntervalPosition(frameNum);

				foreach (IElementNode node in nodes)
				{
					//Color color = colorGradient.GetColorAt(intervalPos);
					//double intensityDbl = ScaleCurveToValue(intensity.GetValue(intervalPos * 100), 1.0, 0);

					_effectIntents.Add(RenderNode(node, intensity, colorGradient));

					//RenderRGBWNode(node, color, string.Empty, intensityDbl, _effectIntents);
										
					//startTime = startTime + frameTs;
				}
			}
		}

		private EffectIntents RenderNode(IElementNode node, Curve levelCurve, ColorGradient colorGradient)
		{			
			return PulseRenderer.RenderNode(node, levelCurve, colorGradient, TimeSpan, false);
		}

		private EffectIntents RenderRGBWNode(IElementNode node, Color color, string tag, double intensity, EffectIntents effectIntents)
		{
			//EffectIntents effectIntents = new EffectIntents();
			var leafs = node.GetLeafEnumerator();
			foreach (IElementNode elementNode in leafs)
			{
				if (HasDiscreteColors && IsElementDiscrete(elementNode))
				{
					IEnumerable<Color> colors = ColorModule.getValidColorsForElementNode(elementNode, false);
					if (!colors.Contains(color))
					{
						continue;
					}
				}

				// JCB - Trying to get intensity to work!
				var intent = CreateIntent(leafs.First(), color, intensity, TimeSpan); //, tag);
				effectIntents.AddIntentForElement(elementNode.Element.Id, intent, TimeSpan.Zero);				
			}

			return effectIntents;
		}
	}
}