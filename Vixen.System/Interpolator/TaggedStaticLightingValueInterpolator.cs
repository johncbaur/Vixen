using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Value;

namespace Vixen.Interpolator
{
	class TaggedStaticLightingValueInterpolator : StaticValueInterpolator<TaggedLightingValue>
	{
		#region Overrides of StaticValueInterpolator<LightingValue>

		/// <inheritdoc />
		protected override TaggedLightingValue InterpolateValue(double percent, TaggedLightingValue startValue, TaggedLightingValue endValue)
		{
			return new TaggedLightingValue(startValue);
		}

		#endregion
	}
}
