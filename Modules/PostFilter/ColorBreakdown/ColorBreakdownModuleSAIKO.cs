using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	
	/// <summary>
	/// This filter gets the intensity percent for a given state for non mixing colors
	/// </summary>
	internal partial class ColorBreakdownFilter : IntentStateDispatch, IBreakdownFilter
	{
		private void HSI_To_RGBW(double H, double S, double I, out double red, out double green, out double blue, out double white)
		{
			int r, g, b, w;
			double cos_h;
			double cos_1047_h;

			H = H % 360.0; // cycle H around to 0-360 degrees
			H = 3.14159 * H / 180.0; // Convert to radians.
			S = S > 0 ? (S < 1 ? S : 1) : 0; // clamp S and I to interval [0,1]
			I = I > 0 ? (I < 1 ? I : 1) : 0;

			if (H < 2.09439)
			{
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				r = (int)(S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				g = (int)(S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				b = 0;
				w = (int)(255 * (1 - S) * I);
			}
			else if (H < 4.188787)
			{
				H = H - 2.09439;
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				g = (int)(S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				b = (int)(S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				r = 0;
				w = (int)(255 * (1 - S) * I);
			}
			else
			{
				H = H - 4.188787;
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				b = (int)(S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				r = (int)(S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				g = 0;
				w = (int)(255 * (1 - S) * I);
			}

			red = r;
			green = g;
			blue = b;
			white = w;
		}

		public static void RGBToHSI(double r, double g, double b, out double h, out double s, out double i)
		{
			i = (r + g + b) / 3.0;

			double rn = r / (r + g + b);
			double gn = g / (r + g + b);
			double bn = b / (r + g + b);

			h = Math.Acos((0.5 * ((rn - gn) + (rn - bn))) / (Math.Sqrt((rn - gn) * (rn - gn) + (rn - bn) * (gn - bn))));
			if (b > g)
			{
				h = 2 * Math.PI - h;
			}

			s = 1 - 3 * Math.Min(rn, Math.Min(gn, bn));
		}

		

		
	
	
	
		private double Constrain(double value, double minimum, double maximum)
		{
			if (value < minimum)
			{
				return minimum;
			}

			if (value > maximum)
			{
				return maximum;
			}

			return value;
		}

		private void hsi_to_rgbw(
			double H, 
			double S, 
			double I, 
			out double red, 
			out double green, 
			out double blue,
			out double white)
		{
			int r = 0;
			int g = 0;
			int b = 0;
			int w = 0;

			double cos_h = 0.0;
			double cos_1047_h = 0.0;

			H = H % 360.0; // cycle H around to 0-360 degrees
			H = 3.14159 * H / 180.0; // Convert to radians.
			S = Constrain(S, 0.0, 1.0);
			I = Constrain(I, 0.0, 1.0);

			if (H < 2.09439)
			{
				cos_h = Math.Cos(H); // Radians
				cos_1047_h = Math.Cos(1.047196667 - H);

				r = (int)(S * 255.0 * I / 3.0 * (1.0 + cos_h / cos_1047_h));
				g = (int) (S * 255.0 * I / 3.0 * (1.0 + (1.0 - cos_h / cos_1047_h)));
				b = 0;
				w = (int)(255.0 * (1.0 - S) * I);
			}
			else if (H < 4.188787)
			{
				H = H - 2.09439;

				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);

				g = (int)(S * 255.0 * I / 3.0 * (1.0 + cos_h / cos_1047_h));
				b = (int)(S * 255.0 * I / 3.0 * (1.0 + (1.0 - cos_h / cos_1047_h)));
				r = 0;
				w = (int)(255.0 * (1.0 - S) * I);
			}
			else
			{
				H = H - 4.188787;
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);

				b = (int) (S * 255.0 * I / 3.0 * (1.0 + cos_h / cos_1047_h));
				r = (int) (S * 255.0 * I / 3.0 * (1.0 + (1.0 - cos_h / cos_1047_h)));
				g = 0;
				w = (int) (255.0 * (1.0 - S) * I);
			}

			//# for some reason, the rgb numbers need to be X3...
			red = Constrain(r * 3, 0, 255);
			green = Constrain(g * 3, 0, 255);
			blue = Constrain(b * 3, 0, 255);
			white = Constrain(w, 0, 255);
		}


		private void HSI_To_RGBW(double H, double S, double I, out double red, out double green, out double blue, out double white)
		{
			int r, g, b, w;
			double cos_h;
			double cos_1047_h;

			H = H % 360.0; // cycle H around to 0-360 degrees
			H = 3.14159 * H / 180.0; // Convert to radians.
			S = S > 0 ? (S < 1 ? S : 1) : 0; // clamp S and I to interval [0,1]
			I = I > 0 ? (I < 1 ? I : 1) : 0;

			if (H < 2.09439)
			{
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				r = (int)(S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				g = (int)(S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				b = 0;
				w = (int)(255 * (1 - S) * I);
			}
			else if (H < 4.188787)
			{
				H = H - 2.09439;
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				g = (int)(S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				b = (int)(S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				r = 0;
				w = (int)(255 * (1 - S) * I);
			}
			else
			{
				H = H - 4.188787;
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				b = (int)(S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				r = (int)(S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				g = 0;
				w = (int)(255 * (1 - S) * I);
			}

			red = r;
			green = g;
			blue = b;
			white = w;
		}

		/*
		public static void RGBToHSI(double r, double g, double b, out double h, out double s, out double i)
		{
			i = (r + g + b) / 3.0;

			double rn = r / (r + g + b);
			double gn = g / (r + g + b);
			double bn = b / (r + g + b);

			h = Math.Acos((0.5 * ((rn - gn) + (rn - bn))) / (Math.Sqrt((rn - gn) * (rn - gn) + (rn - bn) * (gn - bn))));
			if (b > g)
			{
				h = 2 * Math.PI - h;
			}

			s = 1 - 3 * Math.Min(rn, Math.Min(gn, bn));
		}
		*/

		public static void RGBToHSI(double r, double g, double b, out double h, out double s, out double i)
		{
			double[] pixel = { r, g, b };
			double num = 0.5 * (2 * r - g - b);
			double den = Math.Sqrt(Math.Pow(r - g, 2) + (r - b) * (g - b));

			double hue = Math.Acos(num / (den));
			hue = hue * 180.0 / Math.PI;

			if (b > g)
			{
				hue = 2 * Math.PI - hue;
			}

			num = pixel.Min();
			den = r + g + b;

			//if (den == 0)
			//{
			//    den = 0.00000001;
			//}

			double saturation = 1d - 3d * num / den;
			if (saturation == 0)
			{
				hue = 0;
			}

			double intensity = (r + g + b) / 3;

			h = hue;
			s = saturation;
			i = intensity;
		}

		float FilterRGBToRGBW_FilterRed(Color color)
		{
			/*
			double h;
			double s;
			double i;
			RGBToHSI(color.R / 255.0, color.G / 255.0, color.B / 255.0, out h, out s, out i);

			double red;
			double green;
			double blue;
			double white;
			HSI_To_RGBW(h, s, i, out red, out green, out blue, out white);
			*/
			double red;
			double green;
			double blue;
			double white;
			RGBToRGBW(color, out red, out green, out blue, out white);

			return (float)red / 255f;		
		}

		void RGBToRGBW(Color color, out double r, out double g, out double b, out double w)
		{
			float Ri = color.R;
			float Gi = color.G;
			float Bi = color.B;

			//double r;
			//double g;
			//double b;
			//double w;

			float tM = Math.Max(Ri, Math.Max(Gi, Bi));

			//If the maximum value is 0, immediately return pure black.
			if (tM == 0)
			{
				r = 0;
				g = 0;
				b = 0;
				w = 0;
				return;
			}

			//This section serves to figure out what the color with 100% hue is
			float multiplier = 255.0f / tM;
			float hR = Ri * multiplier;
			float hG = Gi * multiplier;
			float hB = Bi * multiplier;

			//This calculates the Whiteness (not strictly speaking Luminance) of the color
			float M = Math.Max(hR, Math.Max(hG, hB));
			float m = Math.Min(hR, Math.Min(hG, hB));
			float Luminance = ((M + m) / 2.0f - 127.5f) * (255.0f / 127.5f) / multiplier;

			//Calculate the output values
			int Wo = Convert.ToInt32(Luminance);
			int Bo = Convert.ToInt32(Bi - Luminance);
			int Ro = Convert.ToInt32(Ri - Luminance);
			int Go = Convert.ToInt32(Gi - Luminance);

			//Trim them so that they are all between 0 and 255
			if (Wo < 0) Wo = 0;
			if (Bo < 0) Bo = 0;
			if (Ro < 0) Ro = 0;
			if (Go < 0) Go = 0;
			if (Wo > 255) Wo = 255;
			if (Bo > 255) Bo = 255;
			if (Ro > 255) Ro = 255;
			if (Go > 255) Go = 255;

			r = Ro;
			g = Go;
			b = Bo;
			w = Wo;
		}

		float FilterRGBToRGBW_FilterGreen(Color color)
		{
			/*
			double h;
			double s;
			double i;
			RGBToHSI(color.R / 255.0, color.G / 255.0, color.B / 255.0, out h, out s, out i);

			double red;
			double green;
			double blue;
			double white;
			HSI_To_RGBW(h, s, i, out red, out green, out blue, out white);
			*/

			double red;
			double green;
			double blue;
			double white;
			RGBToRGBW(color, out red, out green, out blue, out white);

			return (float)green / 255f;
		}
	
		float FilterRGBToRGBW_FilterBlue(Color color)
		{
			/*
			double h;
			double s;
			double i;
			RGBToHSI(color.R / 255.0, color.G / 255.0, color.B /255.0, out h, out s, out i);
			*/

			
			//HSI_To_RGBW(h, s, i, out red, out green, out blue, out white);

			double red;
			double green;
			double blue;
			double white;
			RGBToRGBW(color, out red, out green, out blue, out white);

			return (float)blue / 255f;
		}
	
		float FilterRGBToRGBW_FilterWhite(Color color)
		{
			/*
			double h;
			double s;
			double i;
			RGBToHSI(color.R / 255.0, color.G / 255.0, color.B / 255.0, out h, out s, out i);

			double red;
			double green;
			double blue;
			double white;
			HSI_To_RGBW(h, s, i, out red, out green, out blue, out white);
			*/

			double red;
			double green;
			double blue;
			double white;
			RGBToRGBW(color, out red, out green, out blue, out white);

			return (float)white / 255f;
		}

		public ColorBreakdownMixingFilter(ColorBreakdownItem breakdownItem, bool convertToRGBW)
		{
			if (convertToRGBW)
			{
				if (breakdownItem.Color.Equals(Color.Red))
				{
					_getMaxProportionFunc = FilterRGBToRGBW_FilterRed;
				}
				else if (breakdownItem.Color.Equals(Color.Lime))
				{
					_getMaxProportionFunc = FilterRGBToRGBW_FilterGreen;
				}
				else if (breakdownItem.Color.Equals(Color.Blue))
				{
					_getMaxProportionFunc = FilterRGBToRGBW_FilterBlue;
				}
				else if (breakdownItem.Color.Equals(Color.White))
				{
					_getMaxProportionFunc = FilterRGBToRGBW_FilterWhite;
				}
			}
			else if (breakdownItem.Color.Equals(Color.Red))
			{
				_getMaxProportionFunc = color => color.R / 255f;
			}
			else if (breakdownItem.Color.Equals(Color.Lime))
			{
				_getMaxProportionFunc = color => color.G / 255f;
			}
			else
			{
				_getMaxProportionFunc = color => color.B / 255f;
			}
		}

		/// <summary>
		/// Process the intent and return a value that represents the percent of intensity for the state
		/// </summary>
		/// <param name="intentValue"></param>
		/// <returns></returns>
		public double GetIntensityForState(IIntentState intentValue)
		{
			intentValue.Dispatch(this);
			return _intensityValue;
		}

		private readonly Func<Color, float> _getMaxProportionFunc;

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue value = obj.GetValue();
			_intensityValue = _getMaxProportionFunc(value.FullColor);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			_intensityValue = lightingValue.Intensity * _getMaxProportionFunc(lightingValue.Color);
		}

		public override void Handle(IIntentState<TaggedLightingValue> obj)
		{
			TaggedLightingValue lightingValue = obj.GetValue();
			_intensityValue = lightingValue.Intensity * _getMaxProportionFunc(lightingValue.Color);
		}

	}




	internal class ColorBreakdownOutput : IDataFlowOutput<CommandDataFlowData>
	{
		private readonly IBreakdownFilter _filter;
		private readonly ColorBreakdownItem _breakdownItem;

		public ColorBreakdownOutput(ColorBreakdownItem breakdownItem, bool mixColors, bool convertToRGBW)
		{
			Data = new CommandDataFlowData(CommandLookup8BitEvaluator.CommandLookup[0]); 
			if (mixColors)
			{
				_filter = new ColorBreakdownMixingFilter(breakdownItem, convertToRGBW);
			}
			else
			{
				_filter = new ColorBreakdownFilter(breakdownItem);
			}
			
			_breakdownItem = breakdownItem;
		}

		public void ProcessInputData(IntentsDataFlowData data)
		{
			//Because we are combining at the layer above us, we should really only have one
			//intent that matches this outputs color setting. 
			//Everything else will have a zero intensity and should be thrown away when it does not match our outputs color.
			double intensity = 0;
			if (data.Value?.Count > 0)
			{
				foreach (var intentState in data.Value)
				{
					var i = _filter.GetIntensityForState(intentState);
					intensity = Math.Max(i, intensity);
				}
			}

			Data.Value = CommandLookup8BitEvaluator.CommandLookup[(byte)(intensity * Byte.MaxValue)];
		}

		IDataFlowData IDataFlowOutput.Data => Data;

		public string Name
		{
			get { return _breakdownItem.Name; }
		}

		/// <inheritdoc />
		public CommandDataFlowData Data { get; }
	}
}