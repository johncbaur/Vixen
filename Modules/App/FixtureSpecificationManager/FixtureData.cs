using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Value;
using VixenModules.App.Fixture;

namespace VixenModules.App.FixtureSpecificationManager
{
	public abstract class FixtureData
	{
		static protected FixtureChannel AddChannel(FixtureSpecification specification, string name, string function, int channelNumber)
		{
			FixtureChannel channel = new FixtureChannel();
			channel.Name = name;
			channel.Function = function;
			channel.ChannelNumber = channelNumber;
			specification.ChannelDefinitions.Add(channel);

			return channel;
		}

		static protected void AddColorWheel(FixtureFunction function, string name, int startValue, int endValue, Color color1)
		{
			FixtureColorWheel colorWheel = new FixtureColorWheel();
			colorWheel.Name = name;
			colorWheel.StartValue = startValue;
			//colorWheel.EndValue = endValue;			
			colorWheel.Color1 = color1;
			colorWheel.Color2 = Color.White;
			function.ColorWheelData.Add(colorWheel);
		}

		static protected FixtureFunction AddFunctionType(
			FixtureSpecification specification, 
			string name, 
			FixtureFunctionType functionType,
			FunctionIdentity identity)
		{
			FixtureFunction function = new FixtureFunction();
			function.Name = name;
			function.FunctionType = functionType;
			//function.IncludeInEffect = true;
			function.FunctionIdentity = identity;
			specification.FunctionDefinitions.Add(function);

			return function;
		}

		static protected void AddIndex(FixtureFunction function, string name, int startValue, int endvalue, bool useCurve)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = name;
			enum0.StartValue = startValue;
			enum0.EndValue = endvalue;
			function.IndexData.Add(enum0);
		}
	}
}
