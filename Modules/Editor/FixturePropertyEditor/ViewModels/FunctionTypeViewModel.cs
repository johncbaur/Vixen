using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Maintains and edits a collection of fixture functions.
	/// </summary>
    public class FunctionTypeViewModel : ItemsViewModel<FunctionItemViewModel>
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="functions">Functions associated with the fixture and the function to select initially</param>
        public FunctionTypeViewModel(Tuple<List<FixtureFunction>, string, Action> functions)
		{
			// Create the function item view models and select the specified function
			SelectedItem = InitializeChildViewModels(functions.Item1, functions.Item2);

			// Store off the delegate to refresh the command enable / disable status
			RaiseCanExecuteChanged = functions.Item3;			
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Controls whether the Color Wheel user control is visible.
		/// This control is only visible when the selected function is a color wheel.
		/// </summary>
		public bool ColorWheelVisible
		{
			get
			{
				return GetValue<bool>(ColorWheelVisibleProperty);
			}
			set
			{
				SetValue(ColorWheelVisibleProperty, value);
			}
		}

		/// <summary>
		/// Color Wheel Visible property data.
		/// </summary>
		public static readonly PropertyData ColorWheelVisibleProperty = RegisterProperty(nameof(ColorWheelVisible), typeof(bool), null);

		/// <summary>
		/// Controls whether the Indexed or Enumerated values user control is visible.
		/// This control is only visible when the selected function is an indexed function.
		/// </summary>
		public bool IndexedVisible
		{
			get
			{
				return GetValue<bool>(IndexedVisibleProperty);
			}
			set
			{
				SetValue(IndexedVisibleProperty, value);
			}
		}
		/// <summary>
		/// Indexed visible property data.
		/// </summary>
		public static readonly PropertyData IndexedVisibleProperty = RegisterProperty(nameof(IndexedVisible), typeof(bool), null);

		/// <summary>
		/// Maintains the selected function from the list of fixture functions.
		/// </summary>
		public FunctionItemViewModel SelectedFunction
		{
			get
			{
				return GetValue<FunctionItemViewModel>(SelectedFunctionProperty);
			}
			set
			{
				SetValue(SelectedFunctionProperty, value);
			}
		}

		/// <summary>
		/// Selected function property data.
		/// </summary>
		public static readonly PropertyData SelectedFunctionProperty = RegisterProperty(nameof(SelectedFunction), typeof(FunctionItemViewModel), null);

		/// <summary>
		/// Title for the detailed function group box.  The title on this group box changes as the user selects different functions.
		/// </summary>
		public string GroupBoxTitle
		{
			get
			{
				return GetValue<string>(GroupBoxTitleProperty);
			}
			set
			{
				SetValue(GroupBoxTitleProperty, value);
			}
		}

		/// <summary>
		/// Group box title property data.
		/// </summary>
		public static readonly PropertyData GroupBoxTitleProperty = RegisterProperty(nameof(GroupBoxTitle), typeof(string), null);

		
		/// <summary>
		/// Color Wheel child view model.
		/// </summary>
		public ColorWheelViewModel ColorWheelVM 
		{ 
			get
            {
				ColorWheelViewModel colorWheelVM = null;

				foreach (object vm in GetChildViewModels())
                {
					if (vm is ColorWheelViewModel)
                    {
						colorWheelVM = (ColorWheelViewModel)vm;
					}
                }

				return colorWheelVM;
            }
		}

		/// <summary>
		/// Indexed child view model.
		/// </summary>
		public IndexedViewModel IndexedVM 
		{ 
			get
            {
				IndexedViewModel indexedVM = null;

				foreach (object vm in GetChildViewModels())
				{
					if (vm is IndexedViewModel)
					{
						indexedVM = (IndexedViewModel)vm;
					}
				}

				return indexedVM;
			}				
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates the function item view model objects based on the specified functions.  Returns the function item 
		/// view model object selected initially.
		/// </summary>
		/// <param name="functions">Function model objects to create the view model items from</param>
		/// <param name="functionToSelect">Name of the function to select initially</param>
		/// <returns>Selected function view model object selected initially</returns>
		public FunctionItemViewModel InitializeChildViewModels(List<FixtureFunction> functions, string functionToSelect)
		{
			// Initialize the selected view model object to null
			FunctionItemViewModel itemToSelect = null;
			
			// Loop over the fixture function model objects
			foreach (FixtureFunction functionType in functions)
			{
				// Create a function view model object passing it the index and color data from the model
				FunctionItemViewModel functionVM = new FunctionItemViewModel(functionType.IndexData, functionType.ColorWheelData);

				// Set the function name on the view model object
				functionVM.Name = functionType.Name;

				// If the function is the function to be selected initially then...
				if (functionVM.Name == functionToSelect)
                {
					// Save off this function view model object
					itemToSelect = functionVM;
                }
				
				// Set the function type ( Ranged, Indexed, Color Wheel etc)
				functionVM.FunctionTypeEnum = functionType.FunctionType;
				
				// Set the function identity so that it can be utilized by the preview
				functionVM.FunctionIdentity = functionType.FunctionIdentity;
				
				// Set the function legend so that it can be displayed in the preview
				functionVM.Legend = functionType.Legend;
				
				// Add the function view model to the collection
				Items.Add(functionVM);
			}
			
			// Return the function view model object to select initially
			return itemToSelect;
		}

		/// <summary>
		/// Converts the view model function data back into model data.
		/// </summary>
		/// <returns>Function model data</returns>
		public List<FixtureFunction> GetFunctionData()
		{
			// Create the return collection
			List<FixtureFunction> returnCollection = new List<FixtureFunction>();

			// Loop over all the function view model objects
			foreach (FunctionItemViewModel item in Items)
			{
				// Create a new model function object
				FixtureFunction function = new FixtureFunction();
				
				// Set the function name
				function.Name = item.Name;

				// Set the index data
				function.IndexData = item.IndexData;
				
				// Set the color wheel data
				function.ColorWheelData = item.ColorWheelData;			
				
				// Set the preview legend string
				function.Legend = item.Legend;

				// Set the function type
				function.FunctionType = item.FunctionTypeEnum;

				// Set the function identity for use by the preview
				function.FunctionIdentity = item.FunctionIdentity;

				// Add the model function to the return collection
				returnCollection.Add(function);
			}

			// Return the collection of model functions
			return returnCollection;
		}

		/// <summary>
		/// Returns true if all required function data has been populated.
		/// </summary>
		/// <returns>True if all required function data has been populated</returns>
		public bool CanSave()
		{
			// Check for duplicate function names
			bool canSave = AreFunctionNamesValid();

			// Loop over the function view model items
			foreach (FunctionItemViewModel function in Items)
			{
				// If the function name is empty then...
				if (string.IsNullOrEmpty(function.Name))
				{
					// Inhibit the save 
					canSave = false;
				}
			}

			// Delegate to the color wheel VM and check to see if all color wheel data is complete
			if (ColorWheelVM != null && !ColorWheelVM.IsDataComplete())
			{
				canSave = false;
			}

			// Delegate to the indexed VM and check to see if all indexed data is complete
			if (IndexedVM != null && !IndexedVM.IsDataComplete())
			{
				canSave = false;
			}

			return canSave;
		}

		#endregion
		
        #region Private Methods

		/// <summary>
		/// Returns true if all function names are unique.
		/// </summary>
		/// <returns>True if all function names are unique</returns>
        private bool AreFunctionNamesValid()
        {
			// Default to function names being valid
			bool valid = true;

			// Loop over all the function item VM's
			foreach (FunctionItemViewModel function in Items)
			{
				// If more than one function view model has the same name then...
				if (Items.Count(item => item.Name == function.Name) > 1)
                {
					// Indicate a duplicate function was found!
					valid = false;
                }
			}

			return valid;
		}

		#endregion
	}
}
