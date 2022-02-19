using System;
using System.Collections.Generic;
using System.Windows.Controls;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Maintains a color wheel view.
    /// </summary>
    public partial class ColorWheelView
    {
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		public ColorWheelView(/*List<FixtureColorWheel> colorWheelData*/)
		{
			InitializeComponent();

			//Width = 600;
			//Height = 500;

			//_colorWheelData = colorWheelData;
		}



		#endregion

		#region Private Methods

		/// <summary>
		/// Scrolls the selected color item into view.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Get a reference to the data grid
			DataGrid obj = sender as DataGrid;

			// If there is a selected item then...
			if (obj != null && obj.SelectedItem != null)
			{
				// Scroll the selected into view
				obj.ScrollIntoView(obj.SelectedItem);
			}
		}

        #endregion

        #region Public Methods

		/// <summary>
		/// Retrieves the color wheel data from the view model.
		/// </summary>
		/// <returns>Color wheel model data</returns>
        public List<FixtureColorWheel> GetColorWheelData()
		{
			// Initialize the return value
			List<FixtureColorWheel> colorWheelData = null;

			// If the view model has been created then...
			if (ViewModel != null)
			{
				// Retrieve the color wheel data from the view model
				colorWheelData = (ViewModel as ColorWheelViewModel).GetColorWheelData();
			}

			return colorWheelData;
		}

		/// <summary>
		/// Returns true if the color wheel data is complete.
		/// </summary>
		/// <returns></returns>
		public bool IsDataComplete()
        {
			// Default to the data being complete
			bool isComplete = true;

			// If the view model has been created then...
			if (ViewModel != null)
            {
				// Delegate the request to the view model
				isComplete = (ViewModel as ColorWheelViewModel).IsDataComplete();
			}
			return isComplete;
		}

		/// <summary>
		/// Initializes the color wheel view model with model data.
		/// </summary>
		/// <param name="colorWheelData">Color wheel model data to edit</param>
		/// <param name="raiseCanExecuteChanged">Delegate to refresh command enable/disable status</param>
		public void InitializeViewModel(List<FixtureColorWheel> colorWheelData, Action raiseCanExecuteChanged)
		{
			// If the view model has been created then...
			if (ViewModel != null)
			{
				// Forward the call to the view model
				(ViewModel as ColorWheelViewModel).InitializeChildViewModels(colorWheelData);

				// Give the color wheel VM the ability to refresh the command enable / disable status
				(ViewModel as ColorWheelViewModel).RaiseCanExecuteChanged = raiseCanExecuteChanged;
			}
		}

		#endregion
	}
}
