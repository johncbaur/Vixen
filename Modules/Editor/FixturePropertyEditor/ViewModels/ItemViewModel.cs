using Catel.Data;
using Catel.MVVM;
using System;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Abstract base class for and item view model.  This view model is used with <c>ItemsViewModel</c>.
	/// </summary>
    public abstract class ItemViewModel : ViewModelBase
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public ItemViewModel()
        {
			// Default the name to empty
			Name = String.Empty;
        }

        #endregion

        #region Public Catel Properties

		/// <summary>
		/// Name of the item.
		/// </summary>
        public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string), null);

		#endregion
	}
}
