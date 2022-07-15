﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Data.Value;
using VixenModules.App.Fixture;

namespace VixenModules.App.FixtureSpecificationManager
{
	public class ADJHydroWashX7_17FixtureData : FixtureData
	{		
		static public FixtureSpecification GetFixture()
		{
			FixtureSpecification specification = new FixtureSpecification();

			specification.FunctionDefinitions.Clear();
			specification.ChannelDefinitions.Clear();
			specification.InitializeBuiltInFunctions();
			InitializeX7FunctionTypes(specification);
			InitializeX7Channels(specification);
			return specification;
		}
		
		static private void InitializeX7Channels(FixtureSpecification specification)
		{
			specification.Name = "Hydro Wash X7 (17 Channel Mode)";
			specification.Manufacturer = "American DJ";
			specification.CreatedBy = "Vixen Team";
			specification.Revision = "1.0";

			AddChannel(specification, "Pan Coarse", "Pan", 1);
			AddChannel(specification, "Pan Fine", "Pan", 2);
			AddChannel(specification, "Tilt Coarse", "Tilt", 3);
			AddChannel(specification, "Tilt Fine", "Tilt", 4);
			AddChannel(specification, "Color", "Color Wheel", 5);			
			AddChannel(specification, "Color Macros", "Color Macros", 6);
			AddChannel(specification, "Red-1", "Color", 7);
			AddChannel(specification, "Green-1", "Color", 8);
			AddChannel(specification, "Blue-1", "Color", 9);
			AddChannel(specification, "White-1", "Color", 10);

			/*
			AddChannel(specification, "Red-2", "Color2");               // 11
			AddChannel(specification, "Green-2", "Color2");             // 12
			AddChannel(specification, "Blue-2", "Color2");              // 13
			AddChannel(specification, "White-2", "Color2");             // 14

			AddChannel(specification, "Red-3", "Color3");               // 15
			AddChannel(specification, "Green-3", "Color3");             // 16
			AddChannel(specification, "Blue-3", "Color3");              // 17
			AddChannel(specification, "White-3", "Color3");             // 18

			AddChannel(specification, "Red-4", "Color4");               // 19
			AddChannel(specification, "Green-4", "Color4");             // 20
			AddChannel(specification, "Blue-4", "Color4");              // 21
			AddChannel(specification, "White-4", "Color4");             // 22

			AddChannel(specification, "Red-5", "Color5");               // 23
			AddChannel(specification, "Green-5", "Color5");             // 24
			AddChannel(specification, "Blu+e-5", "Color5");              // 25
			AddChannel(specification, "White-5", "Color5");             // 26

			AddChannel(specification, "Red-6", "Color6");               // 27
			AddChannel(specification, "Green-6", "Color6");             // 28
			AddChannel(specification, "Blue-6", "Color6");              // 29
			AddChannel(specification, "White-6", "Color6");             // 30

			AddChannel(specification, "Red-7", "Color7");               // 31
			AddChannel(specification, "Green-7", "Color7");             // 32
			AddChannel(specification, "Blue-7", "Color7");              // 33
			AddChannel(specification, "White-7", "Color7");             // 34
			*/

			AddChannel(specification, "Shutter", "Shutter", 11);
			AddChannel(specification, "Dimmer Coarse", "Dimmer", 12);
			AddChannel(specification, "Dimmer Fine", "Dimmer", 13);
			AddChannel(specification, "Zoom", "Zoom", 14);
			AddChannel(specification, "Dimmer Modes", "Dimmer Modes", 15);
			AddChannel(specification, "Pan/Tilt Speed", "Pan/Tilt Speed", 16);
			AddChannel(specification, "Function", "Function", 17);
			//AddChannel(specification, "Color Wheel Macros", "Color Wheel Macros", 5);
		}

		static private void InitializeX7ColorWheel(FixtureFunction function)
		{
			AddColorWheel(function, "Open",		0, 2,  Color.FromArgb(0, 0, 0));        // 0 
			AddColorWheel(function, "Color 1",	3, 4,  Color.FromArgb(80, 255, 234));   // 1
			AddColorWheel(function, "Color 2",	5, 6,  Color.FromArgb(80, 255, 164));   // 2
			AddColorWheel(function, "Color 3",	7, 8,  Color.FromArgb(77, 255, 112));   // 3
			AddColorWheel(function, "Color 4",	9, 10, Color.FromArgb(117, 255, 83));  // 4
			AddColorWheel(function, "Color 5",  11, 12, Color.FromArgb(160, 255, 77));  // 5
			AddColorWheel(function, "Color 6",  13, 14, Color.FromArgb(223, 255, 83));  // 6
			AddColorWheel(function, "Color 7",  15, 16, Color.FromArgb(255, 243, 77));  // 7
			AddColorWheel(function, "Color 8",  17, 18, Color.FromArgb(255, 200, 74));  // 8
			AddColorWheel(function, "Color 9",  19, 21, Color.FromArgb(255, 166, 77));  // 9
			AddColorWheel(function, "Color 10", 22, 23, Color.FromArgb(255, 125, 74));  // 10
			AddColorWheel(function, "Color 11", 24, 25, Color.FromArgb(255, 97, 77));   // 11
			AddColorWheel(function, "Color 12", 26, 27, Color.FromArgb(255, 71, 77));   // 12
			AddColorWheel(function, "Color 13", 28, 29, Color.FromArgb(255, 83, 134));  // 13
			AddColorWheel(function, "Color 14", 30, 31, Color.FromArgb(255, 93, 182));  // 14
			AddColorWheel(function, "Color 15", 32, 33, Color.FromArgb(255, 96, 236));  // 15
			AddColorWheel(function, "Color 16", 34, 35, Color.FromArgb(238, 93, 255));    // 16
			AddColorWheel(function, "Color 17", 36, 37, Color.FromArgb(196, 87, 255));    // 17
			AddColorWheel(function, "Color 18", 38, 39, Color.FromArgb(150, 90, 255));    // 18
			AddColorWheel(function, "Color 19", 40, 42, Color.FromArgb(100, 77, 255));    // 19
			AddColorWheel(function, "Color 20", 43, 44, Color.FromArgb(77, 100, 255));    // 20
			AddColorWheel(function, "Color 21", 45, 46, Color.FromArgb(67, 148, 255));    // 21
			AddColorWheel(function, "Color 22", 47, 48, Color.FromArgb(77, 195, 255));    // 22
			AddColorWheel(function, "Color 23", 49, 50, Color.FromArgb(77, 234, 255));    // 23
			AddColorWheel(function, "Color 24", 51, 52, Color.FromArgb(158, 255, 144));   // 24
			AddColorWheel(function, "Color 25", 53, 54, Color.FromArgb(255, 251, 153));   // 25
			AddColorWheel(function, "Color 26", 55, 56, Color.FromArgb(255, 175, 147));  // 26
			AddColorWheel(function, "Color 27", 57, 58, Color.FromArgb(255, 138, 186));  // 27
			AddColorWheel(function, "Color 28", 59, 60, Color.FromArgb(255, 147, 251));  // 28
			AddColorWheel(function, "Color 29", 61, 63, Color.FromArgb(151, 138, 255));  // 29
			AddColorWheel(function, "Indexing", 64, 127, Color.FromArgb(255, 255, 255));  // 30			
			AddColorWheel(function, "Clockwise Rotation Fast to Slow", 128, 189, Color.FromArgb(255, 255, 255));  // 31			
			AddColorWheel(function, "Stop", 190, 193, Color.FromArgb(255, 255, 255));  // 32			
			AddColorWheel(function, "Clockwise Rotation Slow to Fast", 194, 255, Color.FromArgb(255, 255, 255));  // 33			
		}
		
		static void InitializeX7DimModes(FixtureFunction function)
		{
			AddIndex(function, "Standard", 0, 20, false);
			AddIndex(function, "Stage", 21, 40, false);
			AddIndex(function, "Architectural", 61, 80, false);
			AddIndex(function, "Theater", 81, 100, false);
			AddIndex(function, "Stage 2", 101, 120, false);
			AddIndex(function, "Dim Speed From Fast to Slow", 121, 140, true);
			AddIndex(function, "Default to Unit Setting", 141, 255, true);
		}

		static void InitializeX7Function(FixtureFunction function)
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

		static void InitializeX7Shutter(FixtureFunction function)
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
			enum8.IndexType = FixtureIndexType.ShutterOpen;
			function.IndexData.Add(enum8);			
		}

		static void InitializeColorWheelMacro(FixtureFunction function)
		{
			AddIndex(function, "Indexing", 64, 127, true);   // 30			
			AddIndex(function, "Clockwise Rotation Fast to Slow", 128, 189, true);  // 31			
			AddIndex(function, "Stop", 190, 193, true);  // 32			
			AddIndex(function, "Clockwise Rotation Slow to Fast", 194, 255, true);  // 33			
		}

		static private void InitializeX7FunctionTypes(FixtureSpecification specification)
		{
			specification.InitializeBuiltInFunctions();

			FixtureFunction pan = specification.FunctionDefinitions.Single(fn => fn.FunctionIdentity == FunctionIdentity.Pan);
			pan.RotationLimits = new FixtureRotationLimits();
			pan.RotationLimits.StartPosition = 0;
			pan.RotationLimits.StopPosition = 540;

			FixtureFunction tilt = specification.FunctionDefinitions.Single(fn => fn.FunctionIdentity == FunctionIdentity.Tilt);
			tilt.RotationLimits = new FixtureRotationLimits();
			tilt.RotationLimits.StartPosition = 45;
			tilt.RotationLimits.StopPosition = 315;
			
			//AddFunctionType(specification, "Pan", FixtureFunctionType.Range, FunctionIdentity.Pan); // 1
			//AddFunctionType(specification, "Tilt", FixtureFunctionType.Range, FunctionIdentity.Tilt); // 2
			FixtureFunction colorWheel = specification.FunctionDefinitions.Find(fnc => fnc.Name == "Color Wheel");
				//AddFunctionType(specification, "Color Wheel", FixtureFunctionType.ColorWheel, FunctionIdentity.Custom); // 3
			InitializeX7ColorWheel(colorWheel);

			//FixtureFunction colorWheelMacros = AddFunctionType(specification, "Color Wheel Macros", FixtureFunctionType.Indexed, FunctionIdentity.Custom);
			//InitializeColorWheelMacro(colorWheelMacros);

			AddFunctionType(specification, "Color Macros", FixtureFunctionType.Range, FunctionIdentity.Custom); // 4  Really Should be Indexed!
			//AddFunctionType(specification, "Color", FixtureFunctionType.RGBWColor, FunctionIdentity.Custom);   // 5, 6, 7, 8

			//AddFunctionType(specification, "Color2", FixtureFunctionType.RGBWColor);       // 6
			//AddFunctionType(specification, "Color3", FixtureFunctionType.RGBWColor);       // 7
			//AddFunctionType(specification, "Color4", FixtureFunctionType.RGBWColor);       // 8
			//AddFunctionType(specification, "Color5", FixtureFunctionType.RGBWColor);       // 9
			//AddFunctionType(specification, "Color6", FixtureFunctionType.RGBWColor);       // 10
			//AddFunctionType(specification, "Color7", FixtureFunctionType.RGBWColor);       // 11


			FixtureFunction shutter =
				specification.FunctionDefinitions.Find(
					fnc => fnc.FunctionIdentity == FunctionIdentity.Shutter);
				// AddFunctionType(specification, "Shutter", FixtureFunctionType.Indexed, FunctionIdentity.Shutter); // 9
			InitializeX7Shutter(shutter);
			//AddFunctionType(specification, "Dimmer", FixtureFunctionType.Range, FunctionIdentity.Dim); // 10, 11,			
			//AddFunctionType(specification, "Zoom", FixtureFunctionType.Range, FunctionIdentity.Zoom); // 12

			FixtureFunction zoom =
				specification.FunctionDefinitions.Find(
					fnc => fnc.FunctionIdentity == FunctionIdentity.Zoom);

			zoom.ZoomType = FixtureZoomType.WideToNarrow;


			FixtureFunction dimmer = AddFunctionType(specification, "Dimmer Modes", FixtureFunctionType.Indexed, FunctionIdentity.Custom); // 13
			InitializeX7DimModes(dimmer);
			AddFunctionType(specification, "Pan/Tilt Speed", FixtureFunctionType.Range, FunctionIdentity.Custom);                        // 14
			FixtureFunction function = AddFunctionType(specification, "Function", FixtureFunctionType.Indexed, FunctionIdentity.Custom); // 15
			InitializeX7Function(function);
		}
	}
}
