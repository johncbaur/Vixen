using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Value;
using Vixen.Interpolator;

namespace Vixen.Intent
{
	public class TaggedStaticLightingIntent : NonSegmentedLinearIntent<LightingValue>
	{			
		//private static readonly TaggedStaticLightingValueInterpolator Interpolator = new TaggedStaticLightingValueInterpolator();

		/// <inheritdoc />
		public TaggedStaticLightingIntent(LightingValue value, TimeSpan timeSpan, string tag) : base(value, value, timeSpan)
		{
			Tag = tag;
		}

		public string Tag { get; set; }
	}
}
