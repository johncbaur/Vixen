using System;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ShutterFilter
{
	/// <summary>
	/// Descriptor for the Shutter Filter module.
	/// </summary>
	public class ShutterFilterDescriptor : TaggedFilterDescriptorBase<ShutterFilterModule, ShutterFilterData>
	{
		#region Private Static Fields
		
		private static readonly Guid _typeId = new Guid("{72B86092-0A59-4E40-B0A8-9E5FAD0B8EB3}");

		#endregion

		#region Public Static Properties

		public static Guid ModuleId => _typeId;

		#endregion

		#region Public Properties

		public override string TypeName => "Shutter Filter";

		public override Guid TypeId => _typeId;

		public override string Description => "An output filter that converts color intents into shutter commands.";

		#endregion
	}
}