using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vixen.Commands
{
	public class Named8BitCommand : _8BitCommand
	{
		//public NamedCommandType CommandType { get; set; }

		public int IndexType { get; set; }

		public string Tag { get; set; }

		public string Legend { get; set; }

		public Named8BitCommand(byte value) : 
			base(value)
		{			
		}

		public Named8BitCommand(short value) : 
			this((byte)value)
		{
		}

		public Named8BitCommand(int value) : 
			this((byte)value)
		{
		}

		public Named8BitCommand(long value)
			: this((byte)value)
		{
		}

		public Named8BitCommand(float value)
			: this((byte)value)
		{
		}

		public Named8BitCommand(double value)
			: this((byte)value)
		{
		}
	}
}
