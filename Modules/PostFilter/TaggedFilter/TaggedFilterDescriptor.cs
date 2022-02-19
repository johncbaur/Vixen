using System;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Descriptor for the Tagged Filter module.
	/// </summary>
	public class TaggedFilterDescriptor : TaggedFilterDescriptorBase<TaggedFilterModule, TaggedFilterData>
	{
		#region Private Static Properties

		private static readonly Guid _typeId = new Guid("{FE552DCF-1BC0-4588-B23C-2600151D9FC5}");

		#endregion

		#region Public Static Properties

		public static Guid ModuleId => _typeId;

		#endregion

		#region Public Properties

		public override Guid TypeId => _typeId;

		public override string Description => "An output filter that filters intents based on a tag.";

		#endregion
	}
}