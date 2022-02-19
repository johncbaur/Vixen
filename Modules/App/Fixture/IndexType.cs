using System.ComponentModel;
using Vixen.TypeConverters;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Defines the types of indicees to support the preview.
	/// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum FixtureIndexType
	{
		[Description(" ")]
		Custom,

		[Description("Shutter Open")]
		ShutterOpen,

		[Description("Shutter Closed")]
		ShutterClosed,

		[Description("Lamp On")]
		LampOn,

		[Description("Lamp Off")]
		LampOff,

		[Description("Color Wheel")]
		ColorWheel,
	}
}
