using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
	public abstract class TaggedFilterDataBase : ModuleDataModelBase, ITaggedFilterData
	{
		#region Public Properties

		/// <summary>
		/// Tag (function) associated with the filter.
		/// </summary>
		[DataMember]
		public string Tag { get; set; }

		#endregion
	}
}
