using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Maintains a fixture function editor view.
    /// </summary>
    public partial class FunctionTypeView
    {
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="polygonContainer">Polygon/line data</param>
		public FunctionTypeView(/*List<FixtureFunction> functions, string functionToSelect*/)
		{
			InitializeComponent();

			Width = 1100;
			Height = 500;

			// TODO: THIS NEED TO BE PASSED IN!
			//_functions = functions;
			//_functionToSelect = functionToSelect;						
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Flag indicating when index user control is displayed.
		/// </summary>
		private bool _indexDataDisplayed;
		
		/// <summary>
		/// Flag indicating when color wheel user control is displayed.
		/// </summary>
		private bool _colorWheelDataDisplayed;

		/// <summary>
		/// Previously selected function item.
		/// </summary>
		private FunctionItemViewModel _previouslySelectedItem;

		#endregion

		#region Private Methods

		/// <summary>
		/// Event handler for when selected function change is attempted.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			// Default to allowing the selected row to change
			bool allowSelectionToChange = true;

			// If index data is being displayed in the details area then...
			if (_previouslySelectedItem != null && _indexDataDisplayed)
            {
				// Only allow the selection to change if the index data is complete 
				allowSelectionToChange = IndexControl.IsDataComplete();

				// If the selection is changing then...
				if (allowSelectionToChange)
				{
					// Retrieve the index data from the user control
					_previouslySelectedItem.IndexData = IndexControl.GetIndexData();
				}
			}
			
			// If color wheel data is being displayed in the details area then...
			if (_previouslySelectedItem != null && _colorWheelDataDisplayed)
            {
				// Only allow the selection to change if the index data is complete // 
				allowSelectionToChange = ColorWheelControl.IsDataComplete();

				// If the selection is changing then...
				if (allowSelectionToChange)
				{
					// Retrieve the color wheel data from the user control
					_previouslySelectedItem.ColorWheelData = ColorWheelControl.GetColorWheelData();
				}				
			}
			
			// If the selection is changing then...
			if (allowSelectionToChange)
			{ 
				// Get the function data grid from the sender event argument
				DataGrid obj = sender as DataGrid;

				// If there is a selected item then...
				if (obj != null && obj.SelectedItem != null)
				{
					// Scroll the selected item into view
					obj.ScrollIntoView(obj.SelectedItem);

					// Get a reference to the FunctionType view model
					FunctionTypeViewModel vm = (FunctionTypeViewModel)ViewModel;

					if (_previouslySelectedItem != (FunctionItemViewModel)obj.SelectedItem)
					{						// Store off the selected item
						_previouslySelectedItem = (FunctionItemViewModel)obj.SelectedItem;

						// Update the details group box 
						vm.GroupBoxTitle = (ViewModel as FunctionTypeViewModel).SelectedItem.Name + " Details";

						// If the selected item is a color wheel then...
						if ((obj.SelectedItem as FunctionItemViewModel).FunctionTypeEnum == FixtureFunctionType.ColorWheel)
						{
							// Initialize the color wheel control with the color wheel data
							ColorWheelControl.InitializeViewModel(
								(obj.SelectedItem as FunctionItemViewModel).ColorWheelData, 
								(ViewModel as FunctionTypeViewModel).RaiseCanExecuteChanged);

							// Make the color wheel user control visible
							vm.ColorWheelVisible = true;

							// Hide the index user control
							vm.IndexedVisible = false;
										
							// Remember that color wheel data is displayed
							_indexDataDisplayed = false;
							_colorWheelDataDisplayed = true;
						}
						// Otherwise if the selected item is indexed then...
						else if ((obj.SelectedItem as FunctionItemViewModel).FunctionTypeEnum == FixtureFunctionType.Indexed)
						{
							// Initialize the index control with index data
							IndexControl.InitializeViewModel(
								(obj.SelectedItem as FunctionItemViewModel).IndexData,
								(ViewModel as FunctionTypeViewModel).RaiseCanExecuteChanged);
							
							// Make the index user control visible
							(ViewModel as FunctionTypeViewModel).IndexedVisible = true;

							// Hide the color wheel control
							(ViewModel as FunctionTypeViewModel).ColorWheelVisible = false;
													
							// Remember the index data is displayed
							_indexDataDisplayed = true;
							_colorWheelDataDisplayed = false;							
						}
						// Otherwise another type of function was selected
						else
						{
							// Hide both the color wheel and index user controls
							(ViewModel as FunctionTypeViewModel).ColorWheelVisible = false;
							(ViewModel as FunctionTypeViewModel).IndexedVisible = false;

							// Remember that neither color wheel or index data is displayed
							_indexDataDisplayed = false;
							_colorWheelDataDisplayed = false;
						}
					}
				}
			}
			// Not allowing the user to change rows because of incomplete data
			else
            {		
				// Doint this on the dispatcher so that the selection event can complete
				Dispatcher.InvokeAsync(() =>
				{
					// Reselect the prevous item
					functionGrid.SelectedItem = _previouslySelectedItem;

					// Get the data grid row of the previously selected row
					DataGridRow row = functionGrid.ItemContainerGenerator.ContainerFromIndex(functionGrid.SelectedIndex) as DataGridRow;

					// Get the Name column
					DataGridCell cell = GetCell(functionGrid, row, 0);

					// Force focus to the Name column
					cell.Focus();
				});
			}
		}

        #endregion

        #region Microsoft Private Methods

        /// <summary>
        /// Finds a visual child.
        /// </summary>		
        /// <remarks>
        /// This code is from:
        /// https://social.technet.microsoft.com/wiki/contents/articles/21202.wpf-programmatically-selecting-and-focusing-a-row-or-cell-in-a-datagrid.aspx?Sort=MostUseful&PageIndex=1
        /// </remarks>
        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
					return (T)child;
				else
				{
					T childOfChild = FindVisualChild<T>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}
       
		/// <summary>
		/// Retrieves the specified data grid cell.
		/// </summary>
		/// <param name="dataGrid">Datagrid to process</param>
		/// <param name="rowContainer">Row to retrieve the cell from</param>
		/// <param name="column">Column to retrive the cell from</param>
		/// <returns></returns>
        private static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
		{
			if (rowContainer != null)
			{
				DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				if (presenter == null)
				{
					/* if the row has been virtualized away, call its ApplyTemplate() method 
					 * to build its visual tree in order for the DataGridCellsPresenter
					 * and the DataGridCells to be created */
					rowContainer.ApplyTemplate();
					presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				}
				if (presenter != null)
				{
					DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					if (cell == null)
					{
						/* bring the column into view
						 * in case it has been virtualized away */
						dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
						cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					}
					return cell;
				}
			}
			return null;
		}		
    }

	#endregion
}
