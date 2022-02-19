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
	public class ADJHydroBeamX1Data : FixtureData
	{
		static public FixtureSpecification GetFixture()
		{
			FixtureSpecification specification = new FixtureSpecification();

			specification.FunctionDefinitions.Clear();
			specification.ChannelDefinitions.Clear();
			specification.InitializeBuiltInFunctions();
			InitializeX1FunctionTypes(specification);
			InitializeX1Channels(specification);
			return specification;
		}

		public static string Name = "ADJ Hydro Beam X1 (14 Channel Mode)";

		static private void InitializeX1Channels(FixtureSpecification specification)
		{
			specification.Name = Name;
			
			// Channel #1
			FixtureChannel panCourse = new FixtureChannel();
			panCourse.Name = "Pan Course";
			panCourse.Function = "Pan";
			panCourse.ChannelNumber = 1;
			specification.ChannelDefinitions.Add(panCourse);

			// Channel #2
			FixtureChannel panFine = new FixtureChannel();
			panFine.Name = "Pan Fine";
			panFine.Function = "Pan";
			panFine.ChannelNumber = 2;
			specification.ChannelDefinitions.Add(panFine);

			// Channel #3
			FixtureChannel tiltCourse = new FixtureChannel();
			tiltCourse.Function = "Tilt";
			tiltCourse.Name = "Tilt Course";
			tiltCourse.ChannelNumber = 3;
			specification.ChannelDefinitions.Add(tiltCourse);

			// Channel #4
			FixtureChannel tiltFine = new FixtureChannel();
			tiltFine.Name = "Tilt Fine";
			tiltFine.Function = "Tilt";
			tiltFine.ChannelNumber = 4;
			specification.ChannelDefinitions.Add(tiltFine);

			// Channel #5
			FixtureChannel colorWheel = new FixtureChannel();
			colorWheel.Name = "Color Wheel";
			colorWheel.Function = "Color Wheel";
			colorWheel.ChannelNumber = 5;
			specification.ChannelDefinitions.Add(colorWheel);

			// Channel #6
			FixtureChannel gobo = new FixtureChannel();
			gobo.Function = "Gobo";
			gobo.Name = "Gobo";
			gobo.ChannelNumber = 6;
			specification.ChannelDefinitions.Add(gobo);

			// Channel #7
			FixtureChannel prism = new FixtureChannel();
			prism.Function = "Prism";
			prism.Name = "Prism";
			prism.ChannelNumber = 7;
			specification.ChannelDefinitions.Add(prism);

			// Channel #8
			FixtureChannel rotatingPrism = new FixtureChannel();
			rotatingPrism.Function = "Rotating Prism";
			rotatingPrism.Name = "Rotating Prism";
			rotatingPrism.ChannelNumber = 8;
			specification.ChannelDefinitions.Add(rotatingPrism);

			// Channel #9
			FixtureChannel shutterAndStrobing = new FixtureChannel();
			shutterAndStrobing.Name = "Shutter & Strobing";
			shutterAndStrobing.Function = "Shutter & Strobing";
			shutterAndStrobing.ChannelNumber = 9;
			specification.ChannelDefinitions.Add(shutterAndStrobing);

			// Channel #10													
			FixtureChannel dimCoarse = new FixtureChannel();
			dimCoarse.Function = "Dimmer";
			dimCoarse.Name = "Dim Course";
			dimCoarse.ChannelNumber = 10;
			specification.ChannelDefinitions.Add(dimCoarse);

			// Channel #11
			FixtureChannel dimFine = new FixtureChannel();
			dimFine.Function = "Dimmer";
			dimFine.Name = "Dim Fine";
			dimFine.ChannelNumber = 11;
			specification.ChannelDefinitions.Add(dimFine);

			// Channel #12
			FixtureChannel frost = new FixtureChannel();
			frost.Function = "Frost";
			frost.Name = "Frost";
			frost.ChannelNumber = 12;
			specification.ChannelDefinitions.Add(frost);

			// Channel #13
			FixtureChannel movementSpeed = new FixtureChannel();
			movementSpeed.Function = "Pan/Tilt Movement Speed";
			movementSpeed.Name = "Pan/Tilt Movement Speed";
			movementSpeed.ChannelNumber = 13;
			specification.ChannelDefinitions.Add(movementSpeed);

			// Channel #14
			FixtureChannel specialFunction = new FixtureChannel();
			specialFunction.Function = "Special Functions";
			specialFunction.Name = "Special Functions";
			specialFunction.ChannelNumber = 14;
			specification.ChannelDefinitions.Add(specialFunction);
		}

		static private void InitializeX1Shutter(FixtureFunction function)
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

		static private void InitializeSpecialFuntions(FixtureFunction function)
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
			enum7.Name = "Lamp On";
			enum7.StartValue = 130;
			enum7.EndValue = 139;
			enum7.IndexType = FixtureIndexType.LampOn;
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

			FixtureIndex enum10b = new FixtureIndex();
			enum10b.Name = "Reset All Motors";
			enum10b.StartValue = 200;
			enum10b.EndValue = 209;
			function.IndexData.Add(enum10b);

			FixtureIndex enum10c = new FixtureIndex();
			enum10c.Name = "Null 3";
			enum10c.StartValue = 210;
			enum10c.EndValue = 229;
			function.IndexData.Add(enum10c);

			FixtureIndex enum11 = new FixtureIndex();
			enum11.Name = "Lamp Off";
			enum11.StartValue = 230;
			enum11.EndValue = 239;
			enum11.IndexType = FixtureIndexType.LampOff;
			function.IndexData.Add(enum11);

			FixtureIndex enum12 = new FixtureIndex();
			enum12.Name = "Null 4";
			enum12.StartValue = 240;
			enum12.EndValue = 255;
			function.IndexData.Add(enum12);
		}
		static private void InitializePrism(FixtureFunction function)
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

		static private void InitializeRotatingPrism(FixtureFunction function)
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

		static private void InitializeGobos(FixtureFunction function)
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

		// 1. White				255,	255,	255
		// 2. Red				255,	0,		0
		// 3. Blue				  0,	0,		255
		// 4 .Green               0,	255,	0
		// 5. Yellow			255,	255,	0
		// 6. Orange			255,	165,	0	
		// 7. Magenta			255,	0,		255
		// 8. Light Blue		173,	216,	230
		// 9. Light Yellow		255,	255,	224
		// 10. Light Green		144,	238,	144		
		// 11. Purple			128,	0,		128
		// 12. Pink				255,	192,	203
		// 13. Medium Yellow	255,	227,	2


		static private void InitializeColorWheel(FixtureFunction function)
		{
			AddColor(
				function,
				"Open / White",
				0,
				2,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				false,
				false); ;

			AddColor(
				function,
				"White + Red",
				3,
				4,
				System.Drawing.Color.White,
				System.Drawing.Color.Red,
				true,
				false); 

			AddColor(
				function,
				"Red",
				5,
				6,
				System.Drawing.Color.Red,
				System.Drawing.Color.Red,
				false,
				false);

			AddColor(
				function,
				"Red + Blue",
				7,
				8,
				System.Drawing.Color.Red,
				System.Drawing.Color.Blue,
				true,
				false);

			AddColor(
				function,
				"Blue",
				9,
				10,
				System.Drawing.Color.Blue,
				System.Drawing.Color.Blue,
				false,
				false);

			AddColor(
				function,
				"Blue + Green",
				11,
				12,
				System.Drawing.Color.Blue,
				System.Drawing.Color.Green,
				true,
				false);

			AddColor(
				function,
				"Green",
				13,
				14,
				System.Drawing.Color.Green,
				System.Drawing.Color.Green,
				false,
				false);

			AddColor(
				function,
				"Green + Yellow",
				15,
				16,
				System.Drawing.Color.Green,
				System.Drawing.Color.Yellow,
				true,
				false);

			AddColor(
				function,
				"Yellow",
				17,
				18,
				System.Drawing.Color.Yellow,
				System.Drawing.Color.Yellow,
				false,
				false);

			AddColor(
				function,
				"Yellow + Orange",
				19,
				20,
				System.Drawing.Color.Yellow,
				System.Drawing.Color.Orange,
				true,
				false);

			AddColor(
				function,
				"Orange",
				22,
				23,
				System.Drawing.Color.Orange,
				System.Drawing.Color.Orange,
				false,
				false);

			AddColor(
				function,
				"Orange + Magenta",
				24,
				25,
				System.Drawing.Color.Orange,
				System.Drawing.Color.Magenta,
				true,
				false);

			AddColor(
				function,
				"Magenta",
				26,
				27,
				System.Drawing.Color.Magenta,
				System.Drawing.Color.Magenta,
				false,
				false);

			AddColor(
				function,
				"Magenta + Light Blue",
				28,
				29,
				System.Drawing.Color.Magenta,
				System.Drawing.Color.LightBlue,
				true,
				false);

			AddColor(
				function,
				"Light Blue",
				30,
				31,
				System.Drawing.Color.LightBlue,
				System.Drawing.Color.LightBlue,
				false,
				false);

			AddColor(
				function,
				"Light Blue + Light Yellow",
				32,
				33,
				System.Drawing.Color.LightBlue,
				System.Drawing.Color.LightYellow,
				true,
				false);

			AddColor(
				function,
				"Light Yellow",
				34,
				35,
				System.Drawing.Color.LightYellow,
				System.Drawing.Color.LightYellow,
				false,
				false);

			AddColor(
				function,
				"Light Yellow + Light Green",
				36,
				37,
				System.Drawing.Color.LightYellow,
				System.Drawing.Color.LightGreen,
				true,
				false);

			AddColor(
				function,
				"Light Green",
				38,
				39,
				System.Drawing.Color.LightGreen,
				System.Drawing.Color.LightGreen,
				false,
				false);

			AddColor(
				function,
				"Light Green + Purple",
				40,
				42,
				System.Drawing.Color.LightGreen,
				System.Drawing.Color.Purple,
				true,
				false);

			AddColor(
				function,
				"Purple",
				43,
				44,
				System.Drawing.Color.Purple,
				System.Drawing.Color.Purple,
				false,
				false);

			AddColor(
				function,
				"Purple + Pink",
				45,
				46,
				System.Drawing.Color.Purple,
				System.Drawing.Color.Pink,
				true,
				false);

			AddColor(
				function,
				"Pink",
				47,
				48,
				System.Drawing.Color.Pink,
				System.Drawing.Color.Pink,
				false,
				false);

			AddColor(
				function,
				"Pink + Medium Yellow",
				49,
				50,
				System.Drawing.Color.Pink,
				System.Drawing.Color.FromArgb(255, 227, 2), 				
				true,
				false);

			AddColor(
				function,
				"Medium Yellow",
				51,
				52,
				System.Drawing.Color.FromArgb(255, 227, 2),
				System.Drawing.Color.FromArgb(255, 227, 2),
				false,
				false);

			AddColor(
				function,
				"Medium Yellow + CTB",
				53,
				54,
				System.Drawing.Color.FromArgb(255, 227, 2),
				System.Drawing.Color.White,
				true,
				false);

			AddColor(
				function,
				"CTB",
				55,
				56,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				false,
				false);

			AddColor(
				function,
				"CTB + UV",
				57,
				58,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				true,
				false);

			AddColor(
				function,
				"UV",
				59,
				60,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				false,
				false);

			AddColor(
				function,
				"UV + White",
				61,
				63,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				true,
				false);

			AddColor(
				function,
				"Indexing",
				64,
				127,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				false,
				true);				

			AddColor(
				function,
				"Clockwise Rotation Fast-Slow",
				128,
				189,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				false,
				true);

			AddColor(
				function,
				"Stop",
				190,
				193,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				false,
				false);

			AddColor(
				function,
				"Counter-Clockwise Rotation Slow-Fast",
				194,
				255,
				System.Drawing.Color.White,
				System.Drawing.Color.White,
				false,
				true);
		}

		static private void AddColor(
			FixtureFunction function, 
			string name, 
			short startValue, 
			short endValue,
			System.Drawing.Color color1, 
			System.Drawing.Color color2, 
			bool halfStep,
			bool useCurve)
		{
			FixtureColorWheel c = new FixtureColorWheel();
			c.Name = name;
			c.StartValue = startValue;
			//c.EndValue = endValue;
			c.Color1 = Color.FromArgb(color1.ToArgb());
			c.Color2 = Color.FromArgb(color2.ToArgb());
			c.HalfStep = halfStep;
			c.UseCurve = useCurve;
			function.ColorWheelData.Add(c);
		}

		static private void InitializeX1FunctionTypes(FixtureSpecification specification)
		{
			/*
			AddFunctionType(
				specification,
				"Pan",
				FixtureFunctionType.Range,
				FunctionIdentity.Pan);

			AddFunctionType(
				specification,
				"Tilt",
				FixtureFunctionType.Range,
				FunctionIdentity.Tilt);
				*/

			FixtureFunction colorWheel =
				specification.FunctionDefinitions.Single(fnc => fnc.FunctionType == FixtureFunctionType.ColorWheel);
			//	specification,
			//	"Color Wheel",
			//	FixtureFunctionType.ColorWheel,
			//	FunctionIdentity.Custom);			
			InitializeColorWheel(colorWheel);

			FixtureFunction gobo = AddFunctionType(
				specification,
				"Gobo",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Custom);			
			gobo.Legend = "G";
			InitializeGobos(gobo);

			FixtureFunction prism = AddFunctionType(
				specification,
				"Prism",			
				FixtureFunctionType.Indexed,
				FunctionIdentity.Custom);
			prism.Legend = "P";			
			InitializePrism(prism);

			FixtureFunction rotatingPrism = AddFunctionType(
				specification,
				"Rotating Prism",				
				FixtureFunctionType.Indexed,
				FunctionIdentity.Custom);
			rotatingPrism.Legend = "R";			
			InitializeRotatingPrism(rotatingPrism);

			FixtureFunction shutterAndStrobing = AddFunctionType(
				specification,
				"Shutter & Strobing",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Shutter);
			shutterAndStrobing.Legend = "S";
			InitializeShutter(shutterAndStrobing);

			/*
			AddFunctionType(
				specification,
				"Dim",
				FixtureFunctionType.Range,
				FunctionIdentity.Dim);
				*/
			
			AddFunctionType(
				specification,
				"Frost",
				FixtureFunctionType.Range,
				FunctionIdentity.Custom);

			AddFunctionType(
				specification,
				"Pan/Tilt Movement Speed",
				FixtureFunctionType.Range,
				FunctionIdentity.Custom);

			FixtureFunction theDeviceControl = AddFunctionType(
				specification,
				"Special Functions",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Custom);
			InitializeSpecialFuntions(theDeviceControl);
		}

		static private void InitializeShutter(FixtureFunction function)
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
	}
}
