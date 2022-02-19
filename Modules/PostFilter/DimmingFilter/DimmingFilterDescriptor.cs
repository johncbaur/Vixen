using System;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.DimmingFilter
{
	/// <summary>
	/// Descriptor for the Shutter Filter module.
	/// </summary>
	public class DimmingFilterDescriptor : TaggedFilterDescriptorBase<DimmingFilterModule, DimmingFilterData>
	{
		#region Private Static Fields
		
		private static readonly Guid _typeId = new Guid("{61913688-EF44-473C-91D4-D256CCA6CB91}");

		#endregion

		#region Public Static Properties

		public static Guid ModuleId => _typeId;

		#endregion

		#region Public Properties

		public override string TypeName => "Dimming Filter";

		public override Guid TypeId => _typeId;

		public override string Description => "An output filter that converts color intents into dimmer intents.";

		#endregion
	}
}