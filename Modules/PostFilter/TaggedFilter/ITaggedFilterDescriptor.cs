using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.OutputFilter.TaggedFilter
{
	public interface ITaggedFilterDescriptor
	{ 
		string TypeNamePostFix { get; set; }
	}
}
