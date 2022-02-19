using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Asbract base class for a tagged filter descriptor.
	/// </summary>
	/// <typeparam name="TFilterModule">Type of the Tagged filter module</typeparam>
	/// <typeparam name="TFilterData">Type of the Tagged filter data</typeparam>
	public abstract class TaggedFilterDescriptorBase<TFilterModule, TFilterData> : OutputFilterModuleDescriptorBase, ITaggedFilterDescriptor
	{
		#region Public Properties

		public override string TypeName => "Tag Filter";
			//"Tag Filter: " + TypeNamePostFix;

		public override Type ModuleClass => typeof(TFilterModule);

		public override Type ModuleDataClass => typeof(TFilterData);

		public override string Author => "Vixen Team";

		public override string Version => "1.0";

		public string TypeNamePostFix { get; set; }

		#endregion
	}
}
