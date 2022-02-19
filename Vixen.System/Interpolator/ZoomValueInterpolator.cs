using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	[Vixen.Sys.Attribute.Interpolator(typeof(ZoomValue))]
	internal class ZoomValueInterpolator : Interpolator<ZoomValue>
	{
		protected override ZoomValue InterpolateValue(double percent, ZoomValue startValue, ZoomValue endValue)
		{
			double zoom = (startValue.Zoom + (endValue.Zoom - startValue.Zoom) * percent);

			return new ZoomValue(zoom);
		}
	}
}
