using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vixen.Commands
{
	public enum NamedCommandType
	{			
		[Description("Shutter")]
		Shutter,

		[Description("Dim")]
		Dim,

		[Description("Custom")]
		Custom,		
	}
}
