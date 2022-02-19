using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Vixen.Marks;
using VixenModules.App.Fixture;

namespace VixenModules.Effect.Fixture
{
	public class FixtureIndexCollectionNameConverter : TypeConverter
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
			//ObservableCollection<IMarkCollection> markCollections = null;
			//IEmitter emitter = (IEmitter)context.Instance;
			//markCollections = emitter.MarkCollections;

			FixtureFunction fixtureFunction = (FixtureFunction)context.Instance;

			List<string> values = new List<string>();

			foreach (FixtureIndex indexValue in fixtureFunction.IndexData)
			{
				values.Add(indexValue.Name);
			}

			
			//values.Add("Pan");
			//values.Add("Tilt");
			//values.Add("Zoom");
			//values.Add("Prism Rotate");

			//if (markCollections != null)
			//{
			//	foreach (var markCollection in markCollections)
			//	{
			//		values.Add(markCollection.Name);
			//	}
			//}

			return new TypeConverter.StandardValuesCollection(values.ToArray());
		}
	}
}
