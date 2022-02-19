using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Vixen.Marks;
using VixenModules.App.Fixture;

namespace VixenModules.Effect.Fixture
{
	public class ColorIndexConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value?.ToString();
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
			Type destinationType)
		{
			return value.ToString();
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{			
			FixtureFunction fixtureFunction = (FixtureFunction)context.Instance;

			List<string> values = new List<string>();

			foreach (FixtureColorWheel colorIndexValue in fixtureFunction.ColorIndexData)
			{
				values.Add(colorIndexValue.Name);
			}
						
			return new TypeConverter.StandardValuesCollection(values.ToArray());
		}
	}
}
