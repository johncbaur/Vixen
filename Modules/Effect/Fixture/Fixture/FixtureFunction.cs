using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Maintains a waveform for the wave effect.
	/// </summary>
	[ExpandableObject]
	public class FixtureFunction : ExpandoObjectBase, IFixtureFunction
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunction(List<App.Fixture.FixtureFunction> fixtureFunctions)
		{
			//_fixtureSpecifications = fixtureSpecifications;
			_fixtureFunctions = fixtureFunctions;

			FunctionIdentity = FunctionIdentity.Custom;
			Range = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));

			IndexData = new List<FixtureIndex>();
			ColorIndexData = new List<FixtureColorWheel>();

			Color = new ColorGradient(System.Drawing.Color.White);

			InitAllAttributes();
		}

		#endregion

		//List<FixtureSpecification> _fixtureSpecifications;
		List<App.Fixture.FixtureFunction> _fixtureFunctions;

		#region IFixtureRange

		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(15)]
		public ColorGradient Color { get; set; }
		
		/// <summary>
		/// Type of fixture range.
		/// </summary>
		[Browsable(false)]
		public FunctionIdentity FunctionIdentity		
		{
			get;
			set;			
		}

		//[TypeConverter(typeof(RangeTypeCollectionNameConverter))]
		//[PropertyEditor("SelectionEditor")]

		private string _rangeTypeName;

		private bool _enable;

		[ProviderDisplayName(@"Enable")]
		[ProviderDescription(@"Enable")]
		[PropertyOrder(1)]
		public bool Enable
		{
			get
			{
				return _enable;
			}
			set
			{
				_enable = value;
				UpdateFunctionTypeAttributes();
			}
		}
		
		/*
		[Value]
		[ProviderDisplayName(@"Name")]
		[ProviderDescription(@"Name")]
		[PropertyEditor("Label")]
		[PropertyOrder(1)] */
		[Browsable(false)]
		public string FunctionName
		{
			get
			{
				return _rangeTypeName;
			}
			set
			{
				_rangeTypeName = value;				

				/*
				if (_rangeTypeName == "Tilt")
				{
					FunctionIdentity = Vixen.Data.Value.FunctionIdentity.Tilt;
				}
				else if (_rangeTypeName == "Pan")
				{
					FunctionIdentity = Vixen.Data.Value.FunctionIdentity.Pan;
				}
				else if (_rangeTypeName == "Zoom")
				{
					FunctionIdentity = Vixen.Data.Value.FunctionIdentity.Zoom;
				}
				else
				{
					FunctionIdentity = Vixen.Data.Value.FunctionIdentity.Custom;
				}
				*/
			}
		}
		
		/// <summary>
		/// Range of the fixture setting.
		/// </summary>
		[ProviderDisplayName(@"Range")]
		[ProviderDescription(@"Range")]
		[PropertyOrder(2)]
		public Curve Range { get; set; }

		private string _indexValue;

		/// <summary>
		/// Range of the fixture setting.
		/// </summary>
		[ProviderDisplayName(@"Setting")]
		[ProviderDescription(@"Setting")]
		[TypeConverter(typeof(FixtureIndexCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(3)]
		public string IndexValue 
		{ 
			get
			{
				return _indexValue;
			}
			set
			{
				_indexValue = value;

				IndexMapsToRange = false;

				if (!string.IsNullOrEmpty(_indexValue))
				{
					FixtureIndex fixtureIndex = IndexData.Single(item => item.Name == _indexValue);
					if (fixtureIndex.UseCurve)
					{
						IndexMapsToRange = true;
					}
				}

				UpdateFunctionTypeAttributes();
			}
		}

		private string _colorIndexValue;

		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		[TypeConverter(typeof(ColorIndexConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(4)]
		public string ColorIndexValue
		{
			get
			{
				return _colorIndexValue;
			}
			set
			{
				_colorIndexValue = value;
				
				ColorIndexMapsToRange = false;

				if (!string.IsNullOrEmpty(_colorIndexValue))
				{
					FixtureColorWheel fixtureColorWheel = ColorIndexData.Single(item => item.Name == _colorIndexValue);
					if (fixtureColorWheel.UseCurve)
					{
						ColorIndexMapsToRange = true;
					}
				}

				UpdateFunctionTypeAttributes();
			}
		}

		private bool IndexMapsToRange { get; set; }

		private bool ColorIndexMapsToRange { get; set; }

		private FixtureFunctionType _functionType;

		[Browsable(false)]
		public FixtureFunctionType FunctionType 
		{ 
			get
			{
				return _functionType;
			}
			set
			{
				_functionType = value;

				if (_functionType == FixtureFunctionType.Indexed)
				{
					if (_fixtureFunctions.SingleOrDefault(fn => fn.Name == FunctionName) != null)
					{
						// Debugging Copy / Paste
						IndexData = _fixtureFunctions.SingleOrDefault(fn => fn.Name == FunctionName).IndexData;
					}
				}
				if (_functionType == FixtureFunctionType.ColorWheel)
				{		
					if (_fixtureFunctions.SingleOrDefault(fn => fn.Name == FunctionName) != null)
					{ 
						// Debugging Copy / Paste
						ColorIndexData = _fixtureFunctions.SingleOrDefault(fn => fn.Name == FunctionName).ColorWheelData;
					}
				}

				UpdateFunctionTypeAttributes();
			}
		}

		[Browsable(false)]
		public List<FixtureIndex> IndexData { get; set; }

		[Browsable(false)]
		public List<FixtureColorWheel> ColorIndexData { get; set; }

		#endregion

		#region Public Methods

		public override string ToString()
		{
			return FunctionName;
		}

		/// <summary>
		/// Returns a clone of the fixture range.
		/// </summary>		
		public IFixtureFunction CreateInstanceForClone()
		{
			IFixtureFunction result = new FixtureFunction(_fixtureFunctions)
			{
				FunctionIdentity = FunctionIdentity,
				Range = new Curve(Range),
				FunctionName = FunctionName,
				FunctionType = FunctionType,
				IndexValue = IndexValue,
				ColorIndexValue  = ColorIndexValue,				
				Color = new ColorGradient(Color),
				Enable = Enable,
			};

			return result;
		}
		
		#endregion

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return CreateInstanceForClone();
		}
						
		#endregion

		#region Private Methods
		
		/// <summary>
		/// Updates the browseable state of range type properties.
		/// </summary>
		private void UpdateFunctionTypeAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(7)
			{
				{ nameof(IndexValue), Enable && FunctionType == FixtureFunctionType.Indexed },
				{ nameof(Range), Enable && 
				                 FunctionType == FixtureFunctionType.Range || 
				                 IndexMapsToRange || 
								 ColorIndexMapsToRange ||
								 FunctionType == FixtureFunctionType.RGBWColor
				},
				{ nameof(ColorIndexValue), Enable && FunctionType == FixtureFunctionType.ColorWheel },				
				{ nameof(Color), Enable && (FunctionType == FixtureFunctionType.RGBColor || FunctionType == FixtureFunctionType.RGBWColor) },
			};
			SetBrowsable(propertyStates);
		}
		
		/// <summary>
		/// Updates the visibility of the fixture range properties.
		/// </summary>
		private void InitAllAttributes()
		{
			UpdateFunctionTypeAttributes();						
			TypeDescriptor.Refresh(this);
		}

		#endregion
	}
}
