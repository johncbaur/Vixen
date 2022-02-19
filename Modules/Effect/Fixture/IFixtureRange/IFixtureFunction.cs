using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Vixen.Data.Value;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{			
	/// <summary>
	/// Maintains the range properties of a fixture.
	/// </summary>
	public interface IFixtureFunction: ICloneable
	{
		/// <summary>
		/// Range of the fixture setting.
		/// </summary>
		Curve Range { get; set; }

		/// <summary>
		/// Name of the selected range type.
		/// </summary>
		string FunctionName { get; set; }
		
		/// <summary>
		/// Type of fixture range.
		/// </summary>
		FunctionIdentity FunctionIdentity { get; set; }

		FixtureFunctionType FunctionType { get; set; }

		string IndexValue { get; set; }

		string ColorIndexValue { get; set; }
		
		ColorGradient Color { get; set; }

		bool Enable { get; set; }
	}
}
