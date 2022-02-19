using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Fixture;

namespace VixenModules.Property.IntelligentFixture
{
	[DataContract]
	public class IntelligentFixtureData : ModuleDataModelBase
	{
		public IntelligentFixtureData()
		{
		
			//ChannelDefinitions = new List<FixtureChannelData>();
			//FunctionDefinitions = new List<FixtureFunctionData>();
			FixtureSpecification = new FixtureSpecification();
			//InitializeX1Channels();
			InitializeX7Channels(FixtureSpecification);
			InitializeX7FunctionTypes(FixtureSpecification);
		}

		private FixtureFunction AddFunctionType(FixtureSpecification specification, string name, FixtureFunctionType functionType)
		{
			FixtureFunction function = new FixtureFunction();
			function.Name = name;
			function.FunctionType = functionType;
			specification.FunctionDefinitions.Add(function);
			
			return function;
		}

		private void InitializeX7FunctionTypes(FixtureSpecification specification)
		{
			AddFunctionType(specification, "Pan", FixtureFunctionType.Range);              // 1
			AddFunctionType(specification, "Tilt", FixtureFunctionType.Range);             // 2
			FixtureFunction colorWheel = AddFunctionType(specification, "Color Wheel", FixtureFunctionType.ColorWheel); // 3
			InitializeX7ColorWheel(colorWheel);

			AddFunctionType(specification, "Color Macros", FixtureFunctionType.Range);     // 4  Really Should be Indexed!
			AddFunctionType(specification, "Color1", FixtureFunctionType.RGBWColor);        // 5, 6, 7, 8
			
			//AddFunctionType(specification, "Color2", FixtureFunctionType.RGBWColor);       // 6
			//AddFunctionType(specification, "Color3", FixtureFunctionType.RGBWColor);       // 7
			//AddFunctionType(specification, "Color4", FixtureFunctionType.RGBWColor);       // 8
			//AddFunctionType(specification, "Color5", FixtureFunctionType.RGBWColor);       // 9
			//AddFunctionType(specification, "Color6", FixtureFunctionType.RGBWColor);       // 10
			//AddFunctionType(specification, "Color7", FixtureFunctionType.RGBWColor);       // 11


			FixtureFunction shutter = AddFunctionType(specification, "Shutter", FixtureFunctionType.Indexed); // 9
			InitializeX7Shutter(shutter);
			AddFunctionType(specification, "Dimmer", FixtureFunctionType.Range);								// 10, 11,			
			AddFunctionType(specification, "Zoom", FixtureFunctionType.Range);									// 12
			FixtureFunction dimmer = AddFunctionType(specification, "Dimmer Modes", FixtureFunctionType.Range); // 13
			InitializeX7DimModes(dimmer);
			AddFunctionType(specification, "Pan/Tilt Speed", FixtureFunctionType.Range);                        // 14
			FixtureFunction function = AddFunctionType(specification, "Function", FixtureFunctionType.Indexed); // 15
			InitializeX7Function(function);
		}
		

		void InitializeX7DimModes(FixtureFunction function)
		{
			AddIndex(function, "Standard", 0, 20, false);
			AddIndex(function, "Stage", 21, 40, false);
			AddIndex(function, "Architectural", 61, 80, false);
			AddIndex(function, "Theater", 81, 100, false);
			AddIndex(function, "Stage 2", 101, 120, false);
			AddIndex(function, "Dim Speed From Fast to Slow", 121, 140, true);
			AddIndex(function, "Default to Unit Setting", 141, 255, true);
		}

		void InitializeX7Function(FixtureFunction function)
		{
			AddIndex(function, "Normal", 0, 10, false);
			AddIndex(function, "900 Hz LED Refresh Rate", 11, 11, false);
			AddIndex(function, "1000 Hz LED Refresh Rate", 12, 12, false);
			AddIndex(function, "1100 Hz LED Refresh Rate", 13, 13, false);
			AddIndex(function, "1300 Hz LED Refresh Rate", 14, 14, false);
			AddIndex(function, "1400 Hz LED Refresh Rate", 15, 15, false);
			AddIndex(function, "1500 Hz LED Refresh Rate", 16, 16, false);
			AddIndex(function, "2500 Hz LED Refresh Rate", 17, 17, false);
			AddIndex(function, "4000 Hz LED Refresh Rate", 18, 18, false);
			AddIndex(function, "5000 Hz LED Refresh Rate", 19, 19, false);
			AddIndex(function, "6000 Hz LED Refresh Rate", 20, 20, false);
			AddIndex(function, "10000 Hz LED Refresh Rate", 21, 21, false);
			AddIndex(function, "15000 Hz LED Refresh Rate", 22, 22, false);
			AddIndex(function, "Default to Unit Setting", 23, 23, false);
			AddIndex(function, "No Function", 24, 79, false);
			AddIndex(function, "Enable Blackout While Pan/Tilt Moving", 80, 89, false);
			AddIndex(function, "Disable Blackout While Pan/Tilt Moving", 90, 99, false);
			AddIndex(function, "Enable Blackout While Color Changing", 100, 109, false);
			AddIndex(function, "Disable Blackout While Color Changing", 110, 139, false);
			AddIndex(function, "Pan/Tilt Reset", 140, 149, false);
			AddIndex(function, "Zoom Reset", 150, 159, false);
			AddIndex(function, "Null", 160, 199, false);
			AddIndex(function, "All Reset", 200, 209, false);
			AddIndex(function, "Null 2", 210, 239, false);
			AddIndex(function, "No Function 2", 101, 120, false);
		}

		void AddIndex(FixtureFunction function, string name, int startValue, int endvalue, bool useCurve)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = name;
			enum0.StartValue = startValue;
			enum0.EndValue = endvalue;			
			function.IndexData.Add(enum0);			
		}

		void InitializeX7Shutter(FixtureFunction function)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = "Closed";
			enum0.StartValue = 0;
			enum0.EndValue = 31;
			enum0.IndexType = FixtureIndexType.ShutterClosed;
			function.IndexData.Add(enum0);

			FixtureIndex enum1 = new FixtureIndex();
			enum1.Name = "Open 1";
			enum1.StartValue = 32;
			enum1.EndValue = 63;
			enum1.UseCurve = true;
			enum1.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum1);

			FixtureIndex enum2 = new FixtureIndex();
			enum2.Name = "Strobe Effect Slow to Fast";
			enum2.StartValue = 64;
			enum2.EndValue = 95;
			enum2.UseCurve = true;
			function.IndexData.Add(enum2);

			FixtureIndex enum3 = new FixtureIndex();
			enum3.Name = "Open 2";
			enum3.StartValue = 96;
			enum3.EndValue = 127;
			enum3.UseCurve = true;
			enum3.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum3);
			
			FixtureIndex enum5 = new FixtureIndex();
			enum5.Name = "Pulse Effect In Sequences";
			enum5.StartValue = 128;
			enum5.EndValue = 159;
			function.IndexData.Add(enum5);

			FixtureIndex enum6 = new FixtureIndex();
			enum6.Name = "Open 3";
			enum6.StartValue = 160;
			enum6.EndValue = 191;
			enum6.UseCurve = true;
			enum6.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum6);

			FixtureIndex enum7 = new FixtureIndex();
			enum7.Name = "Random Strobe Effect Slow to Fast";
			enum7.StartValue = 192;
			enum7.EndValue = 223;
			enum7.UseCurve = true;
			function.IndexData.Add(enum7);

			FixtureIndex enum8 = new FixtureIndex();
			enum8.Name = "Open 4";
			enum8.StartValue = 224;
			enum8.EndValue = 255;
			enum8.UseCurve = true;
			enum8.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum8);

			FixtureIndex enum9 = new FixtureIndex();
			enum9.Name = "Open 5";
			enum9.StartValue = 248;
			enum9.EndValue = 255;
			enum9.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum9);
		}

		

		private FixtureChannel AddChannel(FixtureSpecification specification, string name, string function)
		{
			FixtureChannel channel = new FixtureChannel();
			channel.Name = name;
			channel.Function = function;
			specification.ChannelDefinitions.Add(channel);
			
			return channel;
		}


		private void InitializeX1Channels()
		{
			FixtureSpecification.Name = "ADJ Hydro Beam X1 (14 Channel Mode)";

			InitializeX1FunctionTypes();

			// Channel #1
			FixtureChannel panCourse = new FixtureChannel();
			panCourse.Name = "Pan Course";
			panCourse.Function = "Pan";			
			FixtureSpecification.ChannelDefinitions.Add(panCourse);

			// Channel #2
			FixtureChannel panFine = new FixtureChannel();
			panFine.Name = "Pan Fine";
			panFine.Function = "Pan";
			FixtureSpecification.ChannelDefinitions.Add(panFine);

			// Channel #3
			FixtureChannel tiltCourse = new FixtureChannel();
			tiltCourse.Function = "Tilt";
			tiltCourse.Name = "Tilt Course";
			FixtureSpecification.ChannelDefinitions.Add(tiltCourse);

			// Channel #4
			FixtureChannel tiltFine = new FixtureChannel();
			tiltFine.Name = "Tilt Fine";
			tiltFine.Function = "Tilt";
			FixtureSpecification.ChannelDefinitions.Add(tiltFine);

			// Channel #5
			FixtureChannel colorWheel = new FixtureChannel();
			colorWheel.Name = "Color Wheel";
			colorWheel.Function = "Color Wheel";
			FixtureSpecification.ChannelDefinitions.Add(colorWheel);

			// Channel #6
			FixtureChannel gobo = new FixtureChannel();
			gobo.Function = "Gobo";
			gobo.Name = "Gobo";			
			FixtureSpecification.ChannelDefinitions.Add(gobo);

			// Channel #7
			FixtureChannel prism = new FixtureChannel();
			prism.Function = "Prism";
			prism.Name = "Prism";			
			FixtureSpecification.ChannelDefinitions.Add(prism);

			// Channel #8
			FixtureChannel rotatingPrism = new FixtureChannel();
			rotatingPrism.Function = "Rotating Prism";
			rotatingPrism.Name = "Rotating Prism";			
			FixtureSpecification.ChannelDefinitions.Add(rotatingPrism);

			// Channel #9
			FixtureChannel shutterAndStrobing = new FixtureChannel();
			shutterAndStrobing.Name = "Shutter & Strobing";
			shutterAndStrobing.Function = "Shutter & Strobing";			
			FixtureSpecification.ChannelDefinitions.Add(shutterAndStrobing);

			// Channel #10													
			FixtureChannel dimCoarse = new FixtureChannel();
			dimCoarse.Function = "Dim";
			dimCoarse.Name = "Dim Course";
			FixtureSpecification.ChannelDefinitions.Add(dimCoarse);

			// Channel #11
			FixtureChannel dimFine = new FixtureChannel();
			dimFine.Function = "Dim";
			dimFine.Name = "Dim Fine";
			FixtureSpecification.ChannelDefinitions.Add(dimFine);

			// Channel #12
			FixtureChannel frost = new FixtureChannel();
			frost.Function = "Frost";
			frost.Name = "Frost";
			FixtureSpecification.ChannelDefinitions.Add(frost);

			// Channel #13
			FixtureChannel movementSpeed = new FixtureChannel();
			movementSpeed.Function = "Pan/Tilt Movement Speed";
			movementSpeed.Name = "Pan/Tilt Movement Speed";
			FixtureSpecification.ChannelDefinitions.Add(movementSpeed);

			// Channel #14
			FixtureChannel specialFunction = new FixtureChannel();
			specialFunction.Function = "Special Functions";
			specialFunction.Name = "Special Functions";			
			FixtureSpecification.ChannelDefinitions.Add(specialFunction);
		}

		//FixtureFunction zoom = new FixtureFunction();
		//zoom.Name = "Zoom";
		//zoom.FunctionType = FixtureFunctionType.Range;
		//FixtureSpecification.FunctionDefinitions.Add(zoom);

		private void InitializeX1FunctionTypes()
		{
			FixtureFunction pan = new FixtureFunction();
			pan.Name = "Pan";
			pan.FunctionType = FixtureFunctionType.Range;
			FixtureSpecification.FunctionDefinitions.Add(pan);

			FixtureFunction tilt = new FixtureFunction();
			tilt.Name = "Tilt";
			tilt.FunctionType = FixtureFunctionType.Range;
			FixtureSpecification.FunctionDefinitions.Add(tilt);

			FixtureFunction colorWheel = new FixtureFunction();
			colorWheel.Name = "Color Wheel";
			colorWheel.FunctionType = FixtureFunctionType.ColorWheel;
			FixtureSpecification.FunctionDefinitions.Add(colorWheel);
			InitializeColorWheel(colorWheel);

			FixtureFunction gobo = new FixtureFunction();
			gobo.Name = "Gobo";
			gobo.FunctionType = FixtureFunctionType.Indexed;
			gobo.Legend = "G";
			InitializeGobos(gobo);
			FixtureSpecification.FunctionDefinitions.Add(gobo);

			FixtureFunction prism = new FixtureFunction();
			prism.Name = "Prism";
			prism.FunctionType = FixtureFunctionType.Indexed;
			prism.Legend = "P";
			FixtureSpecification.FunctionDefinitions.Add(prism);
			InitializePrism(prism);

			FixtureFunction rotatingPrism = new FixtureFunction();
			rotatingPrism.Name = "Rotating Prism";
			rotatingPrism.FunctionType = FixtureFunctionType.Indexed;
			rotatingPrism.Legend = "R";
			FixtureSpecification.FunctionDefinitions.Add(rotatingPrism);
			InitializeRotatingPrism(rotatingPrism);

			FixtureFunction shutterAndStrobing = new FixtureFunction();
			shutterAndStrobing.Name = "Shutter & Strobing";
			shutterAndStrobing.FunctionType = FixtureFunctionType.Indexed;
			shutterAndStrobing.Legend = "S";
			FixtureSpecification.FunctionDefinitions.Add(shutterAndStrobing);
			InitializeShutter(shutterAndStrobing);

			FixtureFunction dim = new FixtureFunction();
			dim.Name = "Dim";
			dim.FunctionType = FixtureFunctionType.Range;
			FixtureSpecification.FunctionDefinitions.Add(dim);

			FixtureFunction frost = new FixtureFunction();
			frost.Name = "Frost";
			frost.FunctionType = FixtureFunctionType.Range;
			FixtureSpecification.FunctionDefinitions.Add(frost);

			FixtureFunction panTiltMovmentSpeed = new FixtureFunction();
			panTiltMovmentSpeed.Name = "Pan/Tilt Movement Speed";
			panTiltMovmentSpeed.FunctionType = FixtureFunctionType.Range;
			FixtureSpecification.FunctionDefinitions.Add(panTiltMovmentSpeed);

			FixtureFunction theDeviceControl = new FixtureFunction();
			theDeviceControl.Name = "Special Functions";
			theDeviceControl.FunctionType = FixtureFunctionType.Indexed;
			FixtureSpecification.FunctionDefinitions.Add(theDeviceControl);
			InitializeSpecialFuntions(theDeviceControl);
		}

		private void InitializeShutter(FixtureFunction function)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = "Shutter Closed";
			enum0.StartValue = 0;
			enum0.EndValue = 31;
			enum0.IndexType = FixtureIndexType.ShutterClosed;
			function.IndexData.Add(enum0);
			
			FixtureIndex enum1 = new FixtureIndex();
			enum1.Name = "Shutter Open";
			enum1.StartValue = 32;
			enum1.EndValue = 63;
			enum1.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum1);

			FixtureIndex enum2 = new FixtureIndex();
			enum2.Name = "Strobing Slow-Fast";
			enum2.StartValue = 64;
			enum2.EndValue = 95;
			enum2.UseCurve = true;
			function.IndexData.Add(enum2);

			FixtureIndex enum3 = new FixtureIndex();
			enum3.Name = "Shutter Open 2";
			enum3.StartValue = 96;
			enum3.EndValue = 127;
			enum3.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum3);

			FixtureIndex enum4 = new FixtureIndex();
			enum4.Name = "Pulse Effect in Sequences";
			enum4.StartValue = 128;
			enum4.EndValue = 159;
			function.IndexData.Add(enum4);

			FixtureIndex enum5 = new FixtureIndex();
			enum5.Name = "Shutter Open 3";
			enum5.StartValue = 160;
			enum5.EndValue = 191;
			enum5.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum5);

			FixtureIndex enum6 = new FixtureIndex();
			enum6.Name = "Random Strobing Slow-Fast";
			enum6.StartValue = 192;
			enum6.EndValue = 223;
			enum6.UseCurve = true;
			function.IndexData.Add(enum6);

			FixtureIndex enum7 = new FixtureIndex();
			enum7.Name = "Shutter Open 4";
			enum7.StartValue = 224;
			enum7.EndValue = 255;
			enum7.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum7);
		}

		private void InitializeSpecialFuntions(FixtureFunction function)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = "Null";
			enum0.StartValue = 0;
			enum0.EndValue = 69;
			function.IndexData.Add(enum0);

			FixtureIndex enum1 = new FixtureIndex();
			enum1.Name = "Enable Blackout with Pan/Tilt Movement";
			enum1.StartValue = 70;
			enum1.EndValue = 79;
			function.IndexData.Add(enum1);

			FixtureIndex enum2 = new FixtureIndex();
			enum2.Name = "Disable Blackout with Pan/Tilt Movement";
			enum2.StartValue = 80;
			enum2.EndValue = 89;
			function.IndexData.Add(enum2);

			FixtureIndex enum3 = new FixtureIndex();
			enum3.Name = "Enable Blackout with Color Change";
			enum3.StartValue = 90;
			enum3.EndValue = 99;
			function.IndexData.Add(enum3);

			FixtureIndex enum4 = new FixtureIndex();
			enum4.Name = "Disable Blackout with Color Change";
			enum4.StartValue = 100;
			enum4.EndValue = 109;
			function.IndexData.Add(enum4);

			FixtureIndex enum5 = new FixtureIndex();
			enum5.Name = "Enable Blackout with Gobo Change";
			enum5.StartValue = 110;
			enum5.EndValue = 119;
			function.IndexData.Add(enum5);

			FixtureIndex enum6 = new FixtureIndex();
			enum6.Name = "Disable Blackout with Gobo Change";
			enum6.StartValue = 120;
			enum6.EndValue = 129;
			function.IndexData.Add(enum6);

			FixtureIndex enum7 = new FixtureIndex();
			enum7.Name = "Lamp On ";
			enum7.StartValue = 130;
			enum7.EndValue = 139;
			function.IndexData.Add(enum7);

			FixtureIndex enum8 = new FixtureIndex();
			enum8.Name = "Reset Pan / Tilt Motors";
			enum8.StartValue = 140;
			enum8.EndValue = 149;
			function.IndexData.Add(enum8);

			FixtureIndex enum9 = new FixtureIndex();
			enum9.Name = "Reset Effect Motor";
			enum9.StartValue = 150;
			enum9.EndValue = 159;
			function.IndexData.Add(enum9);

			FixtureIndex enum10 = new FixtureIndex();
			enum10.Name = "Null 2";
			enum10.StartValue = 160;
			enum10.EndValue = 199;
			function.IndexData.Add(enum10);

			FixtureIndex enum11 = new FixtureIndex();
			enum11.Name = "Lamp Off";
			enum11.StartValue = 160;
			enum11.EndValue = 199;
			function.IndexData.Add(enum11);

			FixtureIndex enum12 = new FixtureIndex();
			enum12.Name = "Null 3";
			enum12.StartValue = 240;
			enum12.EndValue = 255;
			function.IndexData.Add(enum12);
		}

		private void InitializePrism(FixtureFunction function)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = "Close";
			enum0.StartValue = 0;
			enum0.EndValue = 7;
			function.IndexData.Add(enum0);

			FixtureIndex enum1 = new FixtureIndex();
			enum1.Name = "Open";
			enum1.StartValue = 8;
			enum1.EndValue = 255;
			function.IndexData.Add(enum1);		
		}

		private void InitializeRotatingPrism(FixtureFunction function)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = "Indexing";
			enum0.StartValue = 0;
			enum0.EndValue = 127;
			function.IndexData.Add(enum0);

			FixtureIndex enum1 = new FixtureIndex();
			enum1.Name = "Counter-Clockwise Rotation Fast-Slow";
			enum1.StartValue = 128;
			enum1.EndValue = 189;
			enum1.UseCurve = true;
			function.IndexData.Add(enum1);

			FixtureIndex enum2 = new FixtureIndex();
			enum2.Name = "Stop";
			enum2.StartValue = 190;
			enum2.EndValue = 193;			
			function.IndexData.Add(enum2);

			FixtureIndex enum3 = new FixtureIndex();
			enum3.Name = "Clockwise Rotation Slow-Fast";
			enum3.StartValue = 194;
			enum3.EndValue = 255;
			enum3.UseCurve = true;
			function.IndexData.Add(enum3);
		}

		private void InitializeGobos(FixtureFunction function)
		{
			FixtureIndex enum0 = new FixtureIndex();
			enum0.Name = "Open";
			enum0.StartValue = 0;
			enum0.EndValue = 3;
			function.IndexData.Add(enum0);

			FixtureIndex enum1 = new FixtureIndex();
			enum1.Name = "Solid Circle";
			enum1.StartValue = 4;
			enum1.EndValue = 7;
			function.IndexData.Add(enum1);

			FixtureIndex enum2 = new FixtureIndex();
			enum2.Name = "Small Open Circle";
			enum2.StartValue = 8;
			enum2.EndValue = 11;
			function.IndexData.Add(enum2);

			FixtureIndex enum3 = new FixtureIndex();
			enum3.Name = "Medium Open Circle";
			enum3.StartValue = 12;
			enum3.EndValue = 16;
			function.IndexData.Add(enum3);

			FixtureIndex enum4 = new FixtureIndex();
			enum4.Name = "Large Open Circle";
			enum4.StartValue = 16;
			enum4.EndValue = 19;
			function.IndexData.Add(enum4);

			FixtureIndex enum5 = new FixtureIndex();
			enum5.Name = "8 Pt Ninja Star";
			enum5.StartValue = 20;
			enum5.EndValue = 23;

			function.IndexData.Add(enum5);

			FixtureIndex enum6 = new FixtureIndex();
			enum6.Name = "4 Pt Arrow Star";
			enum6.StartValue = 24;
			enum6.EndValue = 27;
			function.IndexData.Add(enum6);

			FixtureIndex enum7 = new FixtureIndex();
			enum7.Name = "Sploch 1";
			enum7.StartValue = 28;
			enum7.EndValue = 31;
			function.IndexData.Add(enum7);

			FixtureIndex enum8 = new FixtureIndex();
			enum8.Name = "Triangle";
			enum8.StartValue = 32;
			enum8.EndValue = 35;
			function.IndexData.Add(enum8);

			FixtureIndex enum9 = new FixtureIndex();
			enum9.Name = "Sploch 2";
			enum9.StartValue = 36;
			enum9.EndValue = 39;
			function.IndexData.Add(enum9);

			FixtureIndex enum10 = new FixtureIndex();
			enum10.Name = "Night Star";
			enum10.StartValue = 40;
			enum10.EndValue = 43;
			function.IndexData.Add(enum10);

			FixtureIndex enum11 = new FixtureIndex();
			enum11.Name = "7 Dots";
			enum11.StartValue = 44;
			enum11.EndValue = 47;
			function.IndexData.Add(enum11);

			FixtureIndex enum12 = new FixtureIndex();
			enum12.Name = "Curved Lines";
			enum12.StartValue = 48;
			enum12.EndValue = 51;
			function.IndexData.Add(enum12);

			FixtureIndex enum13 = new FixtureIndex();
			enum13.Name = "8 Dots";
			enum13.StartValue = 52;
			enum13.EndValue = 55;
			function.IndexData.Add(enum13);

			FixtureIndex enum14 = new FixtureIndex();
			enum14.Name = "Walmart Logo";
			enum14.StartValue = 56;
			enum14.EndValue = 59;
			function.IndexData.Add(enum14);

			FixtureIndex enum15 = new FixtureIndex();
			enum15.Name = "Forward Slash";
			enum15.StartValue = 60;
			enum15.EndValue = 63;
			function.IndexData.Add(enum15);

			//			
			// BEGIN SHAKE
			// 

			FixtureIndex enum16 = new FixtureIndex();
			enum16.Name = "Solid Circle Shake";
			enum16.StartValue = 64;
			enum16.EndValue = 68;
			function.IndexData.Add(enum16);

			FixtureIndex enum17 = new FixtureIndex();
			enum17.Name = "Small Open Circle Shake";
			enum17.StartValue = 69;
			enum17.EndValue = 72;
			function.IndexData.Add(enum17);

			FixtureIndex enum18 = new FixtureIndex();
			enum18.Name = "Medium Open Circle Shake";
			enum18.StartValue = 73;
			enum18.EndValue = 76;
			function.IndexData.Add(enum18);

			FixtureIndex enum19 = new FixtureIndex();
			enum19.Name = "Large Open Circle Shake";
			enum19.StartValue = 77;
			enum19.EndValue = 80;
			function.IndexData.Add(enum19);

			FixtureIndex enum20 = new FixtureIndex();
			enum20.Name = "8 Pt Ninja Star Shake";
			enum20.StartValue = 81;
			enum20.EndValue = 84;
			function.IndexData.Add(enum20);

			FixtureIndex enum21 = new FixtureIndex();
			enum21.Name = "4 Pt Arrow Star Shake";
			enum21.StartValue = 85;
			enum21.EndValue = 92;
			function.IndexData.Add(enum21);

			FixtureIndex enum22 = new FixtureIndex();
			enum22.Name = "Sploch 1 Shake";
			enum22.StartValue = 90;
			enum22.EndValue = 93;
			function.IndexData.Add(enum22);

			FixtureIndex enum23 = new FixtureIndex();
			enum23.Name = "Triangle Shake";
			enum23.StartValue = 94;
			enum23.EndValue = 97;
			function.IndexData.Add(enum23);

			FixtureIndex enum24 = new FixtureIndex();
			enum24.Name = "Sploch 2 Shake";
			enum24.StartValue = 98;
			enum24.EndValue = 101;
			function.IndexData.Add(enum24);

			FixtureIndex enum25 = new FixtureIndex();
			enum25.Name = "Night Star Shake";
			enum25.StartValue = 102;
			enum25.EndValue = 105;
			function.IndexData.Add(enum25);

			FixtureIndex enum26 = new FixtureIndex();
			enum26.Name = "7 Dots Shake";
			enum26.StartValue = 106;
			enum26.EndValue = 110;
			function.IndexData.Add(enum26);

			FixtureIndex enum27 = new FixtureIndex();
			enum27.Name = "Curved Lines Shake";
			enum27.StartValue = 111;
			enum27.EndValue = 114;
			function.IndexData.Add(enum27);

			FixtureIndex enum28 = new FixtureIndex();
			enum28.Name = "8 Dots Shake";
			enum28.StartValue = 115;
			enum28.EndValue = 118;
			function.IndexData.Add(enum28);

			FixtureIndex enum29 = new FixtureIndex();
			enum29.Name = "Walmart Logo Shake";
			enum29.StartValue = 119;
			enum29.EndValue = 122;
			function.IndexData.Add(enum29);

			FixtureIndex enum30 = new FixtureIndex();
			enum30.Name = "Forward Slash Shake";
			enum30.StartValue = 123;
			enum30.EndValue = 127;
			function.IndexData.Add(enum30);

			FixtureIndex enum31 = new FixtureIndex();
			enum31.Name = "Counter-Clockwise Rotation Fast-Slow";
			enum31.StartValue = 128;
			enum31.EndValue = 189;
			enum31.UseCurve = true;
			function.IndexData.Add(enum31);

			FixtureIndex enum32 = new FixtureIndex();
			enum32.Name = "Stop Rotation";
			enum32.StartValue = 190;
			enum32.EndValue = 193;
			function.IndexData.Add(enum32);

			FixtureIndex enum33 = new FixtureIndex();
			enum33.Name = "Clockwise Rotation Slow-Fast";
			enum33.StartValue = 194;
			enum33.EndValue = 255;
			enum33.UseCurve = true;
			function.IndexData.Add(enum33);
		}

		private void InitializeX7ColorWheel(FixtureFunction function)
		{
			AddColorWheel(function, "Open", 0, Color.FromArgb(0, 0, 0));        // 0 
			AddColorWheel(function, "Color 1", 3, Color.FromArgb(80, 255, 234));   // 1
			AddColorWheel(function, "Color 2", 5, Color.FromArgb(80, 255, 164));   // 2
			AddColorWheel(function, "Color 3", 9, Color.FromArgb(77, 255, 112));   // 3
			AddColorWheel(function, "Color 4", 11, Color.FromArgb(117, 255, 83));  // 4
			AddColorWheel(function, "Color 5", 14, Color.FromArgb(160, 255, 77));  // 5
			AddColorWheel(function, "Color 6", 15, Color.FromArgb(223, 255, 83));  // 6
			AddColorWheel(function, "Color 7", 17, Color.FromArgb(255, 243, 77));  // 7
			AddColorWheel(function, "Color 8", 19, Color.FromArgb(255, 200, 74));  // 8
			AddColorWheel(function, "Color 9", 22, Color.FromArgb(255, 166, 77));  // 9
			AddColorWheel(function, "Color 10", 24, Color.FromArgb(255, 125, 74));  // 10
			AddColorWheel(function, "Color 11", 26, Color.FromArgb(255, 97, 77));   // 11
			AddColorWheel(function, "Color 12", 28, Color.FromArgb(255, 71, 77));   // 12
			AddColorWheel(function, "Color 13", 30, Color.FromArgb(255, 83, 134));  // 13
			AddColorWheel(function, "Color 14", 32, Color.FromArgb(255, 93, 182));  // 14
			AddColorWheel(function, "Color 15", 34, Color.FromArgb(255, 96, 236));  // 15
			AddColorWheel(function, "Color 16", 36, Color.FromArgb(238, 93, 255));    // 16
			AddColorWheel(function, "Color 17", 38, Color.FromArgb(196, 87, 255));    // 17
			AddColorWheel(function, "Color 18", 40, Color.FromArgb(150, 90, 255));    // 18
			AddColorWheel(function, "Color 19", 43, Color.FromArgb(100, 77, 255));    // 19
			AddColorWheel(function, "Color 20", 45, Color.FromArgb(77, 100, 255));    // 20
			AddColorWheel(function, "Color 21", 47, Color.FromArgb(67, 148, 255));    // 21
			AddColorWheel(function, "Color 22", 49, Color.FromArgb(77, 195, 255));    // 22
			AddColorWheel(function, "Color 23", 49, Color.FromArgb(77, 234, 255));    // 23
			AddColorWheel(function, "Color 24", 51, Color.FromArgb(158, 255, 144));   // 24
			AddColorWheel(function, "Color 25", 53, Color.FromArgb(255, 251, 153));   // 25
			AddColorWheel(function, "Color 26", 55, Color.FromArgb(255, 175, 147));  // 26
			AddColorWheel(function, "Color 27", 57, Color.FromArgb(255, 138, 186));  // 27
			AddColorWheel(function, "Color 28", 59, Color.FromArgb(255, 147, 251));  // 28
			AddColorWheel(function, "Color 29", 61, Color.FromArgb(151, 138, 255));  // 29

			AddColorWheel(function, "Color 30", 64, Color.FromArgb(151, 138, 255));  // 30
			AddColorWheel(function, "Color 31", 65, Color.FromArgb(138, 169, 255));  // 31
			AddColorWheel(function, "Color 32", 66, Color.FromArgb(255, 255, 255));  // 32			
			AddColorWheel(function, "Color 33", 67, Color.FromArgb(255, 255, 255));  // 32			
			AddColorWheel(function, "Clockwise", 189, Color.FromArgb(255, 255, 255));  // 32			
			AddColorWheel(function, "Stop", 190, Color.FromArgb(255, 255, 255));  // 32			
			AddColorWheel(function, "Counter-Clockwise", 194, Color.FromArgb(255, 255, 255));  // 32			
		}

		private void AddColorWheel(FixtureFunction function, string name, int value, Color color1)
		{
			FixtureColorWheel colorWheel = new FixtureColorWheel();
			colorWheel.Name = name;
			colorWheel.Value = value;
			colorWheel.Color1 = color1;
			colorWheel.Color2 = Color.White;
			function.ColorWheelData.Add(colorWheel);
		}

		private void InitializeColorWheel(FixtureFunction function)
		{			
			FixtureColorWheel c1 = new FixtureColorWheel();
			c1.Name = "Open / White";
			c1.Value = 0;
			c1.Color1 = System.Drawing.Color.White;
			c1.Color2 = System.Drawing.Color.White;
			function.ColorWheelData.Add(c1);
			
			FixtureColorWheel c2 = new FixtureColorWheel();
			c2.Name = "White + Red";
			c2.Value = 3;
			c2.Color1 = System.Drawing.Color.White;
			c2.Color2 = System.Drawing.Color.Red;
			c2.HalfStep = true;
			function.ColorWheelData.Add(c2);

			FixtureColorWheel c3 = new FixtureColorWheel();
			c3.Name = "Red";
			c3.Value = 5;
			c3.Color1 = System.Drawing.Color.Red;
			c3.Color2 = System.Drawing.Color.Red;
			function.ColorWheelData.Add(c3);

			FixtureColorWheel c4 = new FixtureColorWheel();
			c4.Name = "Red + Blue";
			c4.Value = 7;
			c4.Color1 = System.Drawing.Color.Red;
			c4.Color2 = System.Drawing.Color.Blue;
			c4.HalfStep = true;
			function.ColorWheelData.Add(c4);

			FixtureColorWheel c5 = new FixtureColorWheel();
			c5.Name = "Blue";
			c5.Value = 9;
			c5.Color1 = System.Drawing.Color.Blue;
			c5.Color2 = System.Drawing.Color.Blue;
			function.ColorWheelData.Add(c5);

			FixtureColorWheel c6 = new FixtureColorWheel();
			c6.Name = "Blue + Green";
			c6.Value = 11;
			c6.Color1 = System.Drawing.Color.Blue;
			c6.Color2 = System.Drawing.Color.Green;
			c6.HalfStep = true;
			function.ColorWheelData.Add(c6);

			FixtureColorWheel c7 = new FixtureColorWheel();
			c7.Name = "Green";
			c7.Value = 13;
			c7.Color1 = System.Drawing.Color.Green;
			c7.Color2 = System.Drawing.Color.Green;
			function.ColorWheelData.Add(c7);

			FixtureColorWheel c8 = new FixtureColorWheel();
			c8.Name = "Green + Yellow";
			c8.Value = 15;
			c8.Color1 = System.Drawing.Color.Green;
			c8.Color2 = System.Drawing.Color.Yellow;
			c8.HalfStep = true;
			function.ColorWheelData.Add(c8);

			FixtureColorWheel c9 = new FixtureColorWheel();
			c9.Name = "Yellow";
			c9.Value = 17;
			c9.Color1 = System.Drawing.Color.Yellow;
			c9.Color2 = System.Drawing.Color.Yellow;
			function.ColorWheelData.Add(c9);

			FixtureColorWheel c10 = new FixtureColorWheel();
			c10.Name = "Yellow + Orange";
			c10.Value = 19;
			c10.Color1 = System.Drawing.Color.Yellow;
			c10.Color2 = System.Drawing.Color.Orange;
			c10.HalfStep = true;
			function.ColorWheelData.Add(c10);

			FixtureColorWheel c11 = new FixtureColorWheel();
			c11.Name = "Orange";
			c11.Value = 22;
			c11.Color1 = System.Drawing.Color.Orange;
			c11.Color2 = System.Drawing.Color.Orange;
			function.ColorWheelData.Add(c11);

			FixtureColorWheel c12 = new FixtureColorWheel();
			c12.Name = "Orange + Magenta";
			c12.Value = 24;
			c12.Color1 = System.Drawing.Color.Orange;
			c12.Color2 = System.Drawing.Color.Magenta;
			c12.HalfStep = true;
			function.ColorWheelData.Add(c12);

			FixtureColorWheel c13 = new FixtureColorWheel();
			c13.Name = "Magenta";
			c13.Value = 26;
			c13.Color1 = System.Drawing.Color.Magenta;
			c13.Color2 = System.Drawing.Color.Magenta;
			function.ColorWheelData.Add(c13);

			FixtureColorWheel c14 = new FixtureColorWheel();
			c14.Name = "Magenta + Light Blue";
			c14.Value = 22;
			c14.Color1 = System.Drawing.Color.Magenta;
			c14.Color2 = System.Drawing.Color.LightBlue;
			c14.HalfStep = true;
			function.ColorWheelData.Add(c14);

			FixtureColorWheel c15 = new FixtureColorWheel();
			c15.Name = "Light Blue";
			c15.Value = 26;
			c15.Color1 = System.Drawing.Color.LightBlue;
			c15.Color2 = System.Drawing.Color.LightBlue;
			function.ColorWheelData.Add(c15);
		}

		public override IModuleDataModel Clone()
		{
			IntelligentFixtureData clone = new IntelligentFixtureData();

			clone.FixtureSpecification = FixtureSpecification.CreateInstanceForClone();

			/*
			// Clone the Channel Definitions
			for (int index = 0; index < ChannelDefinitions.Count; index++)
			{
				clone.ChannelDefinitions.Add(ChannelDefinitions[index].CreateInstanceForClone());
			}

			for (int index = 0; index < FunctionDefinitions.Count; index++)
			{
				clone.FunctionDefinitions.Add(FunctionDefinitions[index].CreateInstanceForClone());
			}
			*/

			return clone;
		}

		//[DataMember]
		//public List<FixtureChannel> ChannelDefinitions { get; set; }

		//[DataMember]
		//public List<FixtureIndex> FunctionDefinitions { get; set; }

		[DataMember]
		public FixtureSpecification FixtureSpecification { get; set; }
	}
}