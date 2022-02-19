using Catel.Data;
using System.Collections.Generic;
using Vixen.Data.Value;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Maintains a fixture function view model.
    /// </summary>
    public class FunctionItemViewModel : ItemViewModel
	{
		#region Constructors 

		/// <summary>
		/// Constructor
		/// </summary>
		public FunctionItemViewModel()
		{						
			// Default the function type to Range
			FunctionTypeEnum = FixtureFunctionType.Range;

			// Default the preview identity to Custom
			FunctionIdentity = FunctionIdentity.Custom;
			
			// Initialize the index data
			IndexData = new List<FixtureIndex>();

			// Initialize the color wheel data
			ColorWheelData = new List<FixtureColorWheel>();			
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="indexData">Index data associated with the function</param>
		/// <param name="colorWheelData">Color wheel data associated with the function</param>
		public FunctionItemViewModel(
			List<FixtureIndex> indexData,
			List<FixtureColorWheel> colorWheelData) : this()
		{
			// Store off the index data
			IndexData = indexData;

			// Sotre off the color wheel data
			ColorWheelData = colorWheelData;
		}

        #endregion

        #region Public Properties 

		/// <summary>
		/// Index data associated with the function.
		/// </summary>
        public List<FixtureIndex> IndexData { get; set; }

		/// <summary>
		/// Color wheel data associated with the function.
		/// </summary>
		public List<FixtureColorWheel> ColorWheelData { get; set; }

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Type of function (Ranged, Indexed, Color Wheel etc).
		/// </summary>
		public FixtureFunctionType FunctionTypeEnum
		{
			get 
			{ 
				return GetValue<FixtureFunctionType>(FunctionTypeEnumProperty); 
			}
			set 
			{ 
				SetValue(FunctionTypeEnumProperty, value);							
			}
		}

		/// <summary>
		/// Function type property data.
		/// </summary>
		public static readonly PropertyData FunctionTypeEnumProperty = RegisterProperty(nameof(FunctionTypeEnum), typeof(FixtureFunctionType), null);

		/// <summary>
		/// Function (preview) identity.
		/// </summary>
		public FunctionIdentity FunctionIdentity
		{
			get
			{
				return GetValue<FunctionIdentity>(FunctionIdentityProperty);
			}
			set
			{
				SetValue(FunctionIdentityProperty, value);
			}
		}

		/// <summary>
		/// Function (preview) property data.
		/// </summary>
		public static readonly PropertyData FunctionIdentityProperty = RegisterProperty(nameof(FunctionIdentity), typeof(FunctionIdentity), null);
		
		/// <summary>
		/// Preview legend associated with the function.
		/// </summary>
		public string Legend
		{
			get { return GetValue<string>(LegendProperty); }
			set { SetValue(LegendProperty, value); }
		}

		/// <summary>
		/// Legend property data.
		/// </summary>
		public static readonly PropertyData LegendProperty = RegisterProperty(nameof(Legend), typeof(string), null);

		#endregion
	}
}
