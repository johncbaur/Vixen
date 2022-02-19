using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
	/// <summary>
	/// Maintains fixture index data view.
	/// </summary>
    public partial class IndexedView
    {
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>		
		public IndexedView()
		{
			// Initialize the user control
			InitializeComponent();
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
		/// Gets the edited fixture index data.
		/// </summary>
		/// <returns>Edited index data</returns>
        public List<FixtureIndex> GetIndexData()
		{
			// Declare the return value
			List<FixtureIndex> indexData = null;

			// If the view model has been populated then...
			if (ViewModel != null)
			{
				// Retrieve the index data from the view model
				indexData = (ViewModel as IndexedViewModel).GetIndexData();
			}

			return indexData;
		}

		#endregion
		
		/// <summary>
		/// Initializes the view model with index model data.
		/// </summary>
		/// <param name="indexData">Index model data</param>
		public void InitializeViewModel(List<FixtureIndex> indexData, Action raiseCanExecuteChanged)
        {
			// If the view model has been created then...
			if (ViewModel != null)
			{
				// Forward the index model data to the view model
				(ViewModel as IndexedViewModel).InitializeChildViewModels(indexData);

				// Give the index VM the ability to refresh the command enable / disable status
				(ViewModel as IndexedViewModel).RaiseCanExecuteChanged = raiseCanExecuteChanged;
			}
		}



		public bool IsDataComplete()
		{
			bool dataIsComplete = true;

			if (ViewModel != null)
			{
				dataIsComplete = (ViewModel as IndexedViewModel).IsDataComplete();
			}

			return dataIsComplete;
		}
	}
}
