using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class ZoomIntent : NonSegmentedLinearIntent<ZoomValue>
	{
		#region Constructor

		public ZoomIntent(ZoomValue startValue, ZoomValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{			
		}

		#endregion
	}
}
