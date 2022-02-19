using System;
using Catel.Data;
using Catel.MVVM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.Editor.FixturePropertyEditor.Views;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// View model for a fixture specification.
    /// </summary>
    public class FixturePropertyEditorViewModel : ItemsViewModel<ChannelItemViewModel>
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fixtureSpecification">Fixture specification being edited</param>
        public FixturePropertyEditorViewModel(Tuple<FixtureSpecification, Action> fixtureTuple)
		{			
			// Create the collection of function names
			Functions = new ObservableCollection<string>();

			// Create button commands
			EditFunctionsCommand = new Command(EditFunctions, CanEditFunctions);
			LoadSpecificationCommand = new Command(LoadSpecification);
			SaveSpecificationCommand = new Command(SaveSpecification, CanSave);

			// Create the collection of allowable channel numbers
			_channelNumbers = new ObservableCollection<string>();

			// Initialize the channel view models	
			InitializeChildViewModels(fixtureTuple.Item1);

			// Save off the action that refreshes commands
			RaiseCanExecuteChanged = fixtureTuple.Item2;
		}

		#endregion

		#region Public Properties

		public ICommand OKCommand { get; set; }

		/// <summary>
		/// Command for editing the functions.
		/// </summary>
		public ICommand EditFunctionsCommand { get; set; }

		/// <summary>
		/// Command for loading an existing fixture specification.
		/// </summary>
		public ICommand LoadSpecificationCommand { get; set; }

		/// <summary>
		/// Command for saving the fixture specfication to the global Vixen repository.
		/// </summary>
		public ICommand SaveSpecificationCommand { get; set; }

		/// <summary>
		/// Collection of fixture function names.
		/// </summary>
		public ObservableCollection<string> Functions
		{
			get
			{
				return GetValue<ObservableCollection<string>>(FunctionsProperty);
			}
			set
			{
				SetValue(FunctionsProperty, value);
			}
		}
		
		/// <summary>
		/// Function names property data.
		/// </summary>
		public static readonly PropertyData FunctionsProperty = RegisterProperty(nameof(Functions), typeof(ObservableCollection<string>), null);

		/// <summary>
		/// Name of the fixture.
		/// </summary>
		public string Name
		{
			get 
			{ 
				return GetValue<string>(NameProperty); 
			}
			set 
			{ 
				SetValue(NameProperty, value); 
			}
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string), null);

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns true if all required data has been populated and it is safe to save the data.
		/// This method is being used to determine if it is safe to either save the property data or
		/// to save the fixture specification to the Vixen repository.
		/// </summary>
		/// <returns>True if all required data has been poopulated</returns>
		public bool CanSave()
        {
			// Default to not being valid to save
			bool canSave = false;

			// Make sure the Name and Items collection have been created
			if (Name != null && Items != null)
			{
				// Can only save if the fixture has been assigned a name
				canSave = Name.Length > 0;

				// A fixture is required to have at least one channel
				canSave &= Items.Count > 0;

				// Loop over all the channel view models
				foreach(ChannelItemViewModel channel in Items)
                {
					// If the channel function has been tagged None then...
					if (channel.Function == "None")
                    {
						// This channel is saveable
						canSave = true;
                    }
					else
					{
						// Can only save if the channel name is NOT empty
						canSave &= channel.Name != null && channel.Name.Length > 0;

						// Can only save if the asoociated function name is NOT empty
						canSave &= channel.Function != null && channel.Function.Length > 0;
					}
				}
			}

			return canSave;
        }

		/// <summary>
		/// Initializes the channel view model items.
		/// </summary>
		/// <param name="fixtureSpecification">Fixture specification being edited.</param>
		public void InitializeChildViewModels(FixtureSpecification fixtureSpecification)
		{
			// Save off a copy of the fixture specification
			_fixtureSpecification = fixtureSpecification.CreateInstanceForClone();
					
			// Set the name of the fixture
			Name = fixtureSpecification.Name;

			// Update the function name collection
			UpdateFunctionNames(_fixtureSpecification.FunctionDefinitions);

			// Clear the channel items view model collection
			Items.Clear();

			// Loop over the model channel definitions		
			foreach (FixtureChannel fixtureChannel in fixtureSpecification.ChannelDefinitions)
			{
				// Create a new channel view model item
				ChannelItemViewModel channelItem = CreateNewItem();

				// Set the channel number
				channelItem.ChannelNumber = fixtureChannel.ChannelNumber.ToString(); 

				// Set the channel name
				channelItem.Name = fixtureChannel.Name;

				// Set the function associated with the channel
				channelItem.Function = fixtureChannel.Function;
		
				// Add the view model item to the Items collection
				Items.Add(channelItem);
			}
		}

		/// <summary>
		/// Gets the fixture specification including any edits.
		/// </summary>
		/// <returns>Fixture specification</returns>
		public FixtureSpecification GetFixtureSpecification()
		{
			// Transfer the name back to the model
			_fixtureSpecification.Name = Name;

			// Clear the channel defintions in the model
			_fixtureSpecification.ChannelDefinitions.Clear();

			// Loop over the channel definitions in the view model
			foreach (ChannelItemViewModel channelVM in Items)
			{
				// Create a new fixture channel
				FixtureChannel fixtureChannel = new FixtureChannel();

				// Add the channel definition to the fixture
				_fixtureSpecification.ChannelDefinitions.Add(fixtureChannel);

				// Copy of the name of the channel from the view model to the model
				fixtureChannel.Name = channelVM.Name;

				// Copy the function associated with the channel
				fixtureChannel.Function = channelVM.Function;

				// Copy the channel number
				fixtureChannel.ChannelNumber = int.Parse(channelVM.ChannelNumber);
			}

			return _fixtureSpecification;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// <inheritdoc/>  Refer to base class documentation.
		/// </summary>
		/// <returns></returns>
		protected override ChannelItemViewModel CreateNewItem()
		{
			// Create a new channel view model
			ChannelItemViewModel channel = new ChannelItemViewModel(Functions);

			// Give the channel access to the edit functions command
			channel.EditFunctionsCommand = EditFunctionsCommand;

			// Give the channel the allowable channel numbers
			channel.ChannelNumbers = _channelNumbers;

			// Default the channel number to the next channel number to the allowable channel numbers
			string nextChannelNumber = (Items.Count() + 1).ToString();

			// Don't insert duplicates into the channel numbers
			if (!_channelNumbers.Contains(nextChannelNumber))
			{
				// Add the channel number to the allowable channel numbers					
				_channelNumbers.Add(nextChannelNumber);
			}

			// Initialize the channel number 
			channel.ChannelNumber = nextChannelNumber;

			// Return the new channel VM
			return channel;
		}

		/// <summary>
		/// <inheritdoc/> Refer to base class documentation.
		/// </summary>
		protected override void AddItem()
		{
			// Call the base class implementation
			base.AddItem();

			//
			// TODO: I DON"T THINK I NEED THIS CODE!!!
			//
			//Items[Items.Count - 1].Functions = Functions;

			// Initialize the channel number to the next channel
			// JCB 1/14/22
			//Items[Items.Count - 1].ChannelNumber = Items.Count.ToString();
		}

        #endregion

        #region Fields

		/// <summary>
		/// Collection of allowable channel numbers.
		/// </summary>
        private ObservableCollection<string> _channelNumbers;

        /// <summary>
		/// Fixture specification being edited.
		/// </summary>
		private FixtureSpecification _fixtureSpecification;

		#endregion
		
		#region Private Methods

		/// <summary>
		/// Returns true when functions can be edited.
		/// </summary>
		/// <returns>True when functions can be edited.</returns>
		private bool CanEditFunctions()
		{
			// Functions are always editable
			return true;
		}

		/// <summary>
		/// Updates the function name collection from the model collection.
		/// </summary>
		/// <param name="functionData"></param>
		private void UpdateFunctionNames(List<FixtureFunction> functionData)
        {			
			// Create a new observable collection for the function names
			ObservableCollection<string> updatedFunctions = new ObservableCollection<string>();

			// Populate the function name collection from the model data 
			foreach (FixtureFunction function in functionData)
			{
				// Add the function name to the collection
				updatedFunctions.Add(function.Name);
			}

			// Replace the collection of functions
			Functions = updatedFunctions;
		}

		/// <summary>
		/// Edits the functions associated with this fixture.
		/// </summary>
		private void EditFunctions()
		{	
			// Create the function type editor window indicating which function to select
			FunctionTypeWindowView view = new FunctionTypeWindowView(_fixtureSpecification.FunctionDefinitions, SelectedItem.Function);

			// Display the function type editor window
			bool? result = view.ShowDialog();

			// If the user selected to commit the function changes then...
			if (result.Value)
			{
				// Retrieve the function data
				_fixtureSpecification.FunctionDefinitions = view.GetFunctionData();
				
				// Update the function name collection
				UpdateFunctionNames(_fixtureSpecification.FunctionDefinitions);
				
				// Loop over the channel items looking for deleted functions
				foreach (ChannelItemViewModel channel in Items)
				{
					// If the function associated with channel is no longer found then...
					if (Functions.SingleOrDefault(x => x == channel.Function) == null)
					{
						// Set the channel function to 'None'
						channel.Function = "None";
					}
				}
			}
		}

		/// <summary>
		/// Load Specification command handler, replaces the properties fixture specification from the copy in the Vixen fixture repository.
		/// </summary>
		private void LoadSpecification()
		{		
			// Create the view for selecting a fixture specification
			SelectFixtureSpecificationView view = new SelectFixtureSpecificationView(FixtureSpecificationManager.Instance().FixtureSpecifications);

			// Display the selection dialog
			bool? result = view.ShowDialog();

			// If the user picked a fixture specification then...
			if (result.Value)
			{
				// Retrieve the selected fixture specification
				FixtureSpecification fixtureSpecification = view.GetSelectedFixtureSpecification();
			
				// Update the view model with the new fixture specification
				InitializeChildViewModels(fixtureSpecification);
			}

			((Command)(ParentViewModel as FixturePropertyEditorWindowViewModel).OkCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Save Specification command handler, saves the fixture specification to the Vixen repository.
		/// </summary>
		private void SaveSpecification()
        {
			// Make sure the model is up to date with all edits in the view model
			_fixtureSpecification = GetFixtureSpecification();

			// Save the fixture to the Vixen respository
			FixtureSpecificationManager.Instance().Save(_fixtureSpecification);
		}

		#endregion
	}
}
