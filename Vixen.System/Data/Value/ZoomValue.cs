using System;

namespace Vixen.Data.Value
{
	public struct ZoomValue : IIntentDataType
	{
		public ZoomValue(double zoom)
		{
			if (zoom < 0 || zoom > 1) throw new ArgumentOutOfRangeException(nameof(zoom));

			Zoom = zoom;			
		}

		/// <summary>
		/// Zoom value between 0 and 1.
		/// </summary>
		public double Zoom;		
	}
}
