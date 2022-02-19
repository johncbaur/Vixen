using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Abstract base class for a for model that maintains a collection of items.
    /// </summary>
    /// <typeparam name="TItemType">Type of item in the collection</typeparam>
    public abstract class ItemsViewModel<TItemType> : ViewModelBase
		where TItemType : class, new()
	{
        #region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
        public ItemsViewModel()
		{
			// Create the Add button command
			AddCommand = new Command(AddItem, CanAddItem);

			// Create the Delete button commmand
			DeleteCommand = new Command(DeleteItem, CanDeleteItem);

			// Create the collection of items
			Items = new ObservableCollection<TItemType>();
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// Command for adding a new item.
		/// </summary>
        public ICommand AddCommand { get; private set; }
		
		/// <summary>
		/// Command for deleting an item.
		/// </summary>
		public ICommand DeleteCommand { get; private set; }

		/// <summary>
		/// Delegate to ensure the OK button is enabled or disabled promptly.
		/// </summary>
		public Action RaiseCanExecuteChanged { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Determines if an item can be deleted.
		/// </summary>
		/// <returns>True if an item can be deleted</returns>
		protected virtual bool CanDeleteItem()
		{
			// By default items can deleted
			return true;
		}

		/// <summary>
		/// Determines if an item can be added.
		/// </summary>
		/// <returns>True if an item can be added</returns>
		protected virtual bool CanAddItem()
		{
			// By default items can be added
			return true;
		}

		/// <summary>
		/// Deletes the currently selected item.
		/// </summary>
		protected void DeleteItem()
		{
			// If an item is selected then...
			if (SelectedItem != null)
			{
				// Remove the item
				Items.Remove(SelectedItem);

				// Clear out the selected item
				SelectedItem = null;

				// Refresh command enable/disable status
				RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Adds a new item.
		/// </summary>
		protected virtual void AddItem()
		{
			// Create the new item
			TItemType item = CreateNewItem();

			// Add the item to the collection
			Items.Add(item);

			// Select the item
			SelectedItem = item;

			// Refresh command enable/disable status
			RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Creates a new item.
		/// </summary>
		/// <returns>Newly created item</returns>
		protected virtual TItemType CreateNewItem()
		{
			// Create a new item
			return new TItemType();
		}

        #endregion

        #region Public Catel Properties

		/// <summary>
		/// Selected item property.
		/// </summary>
        public TItemType SelectedItem
		{
			get { return GetValue<TItemType>(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }			
		}

		/// <summary>
		/// Selected item property data.
		/// </summary>
		public static readonly PropertyData SelectedItemProperty = RegisterProperty(nameof(SelectedItem), typeof(TItemType), null);

        #endregion

        #region Public Properties

		/// <summary>
		/// Collection of items maintained by the view model.
		/// </summary>
        public ObservableCollection<TItemType> Items { get; set; }

		#endregion
	}
}
