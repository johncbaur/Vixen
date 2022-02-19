using System.Windows.Controls;
using Catel.MVVM.Views;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Maintains the fixture property editor view.
    /// </summary>
    public partial class FixturePropertyEditorView
    {
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		public FixturePropertyEditorView()
		{
			InitializeComponent();								
		}

		#endregion

		#region Public Methods

		/*
		public void InitializeViewModel(Action raiseCanExecuteChanged)
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
		*/
		#endregion

		#region Private Methods

		/// <summary>
		/// Scrolls the selected item into view.
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
	}
}
