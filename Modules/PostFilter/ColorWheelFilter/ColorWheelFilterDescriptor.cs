using System;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ColorWheelFilter
{
	/// <summary>
	/// Descriptor for the Color Wheel Filter module.
	/// </summary>
	public class ColorWheelFilterDescriptor : TaggedFilterDescriptorBase<ColorWheelFilterModule, ColorWheelFilterData>
	{
		#region Private Static Fields
		
		private static readonly Guid _typeId = new Guid("{18470C0C-1E75-464F-BD9F-A3E241CD1EB8}");

		#endregion

		#region Public Static Properties

		public static Guid ModuleId => _typeId;

		#endregion

		#region Public Properties

		public override string TypeName => "Color Wheel Filter";

		public override Guid TypeId => _typeId;

		public override string Description => "An output filter that converts color intents into color wheel index commands.";

		#endregion
	}
}