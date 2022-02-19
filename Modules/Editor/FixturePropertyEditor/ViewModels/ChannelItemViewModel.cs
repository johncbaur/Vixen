using Catel.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Maintains a channel (view model) of the fixture.
	/// </summary>
    public class ChannelItemViewModel : ItemViewModel
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ChannelItemViewModel()
        {
        }

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="functions">Function names defined for the fixture</param>
		public ChannelItemViewModel(ObservableCollection<string> functions)
		{
			// Store off the function names
			Functions = functions;
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// Command to display a dialog for editing the functions.
		/// </summary>
        public ICommand EditFunctionsCommand { get; set; }

		/// <summary>
		/// Collection of available function names.
		/// </summary>
		public ObservableCollection<string> Functions { get; set; }

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Gets or sets the allowable channel numbers.
		/// </summary>
		public ObservableCollection<string> ChannelNumbers
		{
			get { return GetValue<ObservableCollection<string>>(ChannelNumbersProperty); }
			set { SetValue(ChannelNumbersProperty, value); }
		}

		/// <summary>
		/// Channel numbers property data.
		/// </summary>
		public static readonly PropertyData ChannelNumbersProperty = RegisterProperty(nameof(ChannelNumbers), typeof(ObservableCollection<string>), null);


		/// <summary>
		/// Gets or sets the channel number.
		/// </summary>
		public string ChannelNumber
		{
			get { return GetValue<string>(ChannelNumberProperty); }
			set { SetValue(ChannelNumberProperty, value); }
		}

		/// <summary>
		/// Channel number property data.
		/// </summary>
		public static readonly PropertyData ChannelNumberProperty = RegisterProperty(nameof(ChannelNumber), typeof(string), null);

		/// <summary>
		/// Function associated with the channel.
		/// </summary>
		public string Function
		{
			get { return GetValue<string>(FunctionProperty); }
			set { SetValue(FunctionProperty, value); }
		}

		/// <summary>
		/// Function property data.
		/// </summary>
		public static readonly PropertyData FunctionProperty = RegisterProperty(nameof(Function), typeof(string), null);

		#endregion		
	}
}