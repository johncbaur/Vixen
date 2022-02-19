using System.Collections.Generic;
using VixenModules.App.Fixture;
using VixenModules.OutputFilter.ColorWheelFilter.Outputs;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ColorWheelFilter
{
	/// <summary>
	/// Maintains a color wheel filter module.
	/// </summary>
	public class ColorWheelFilterModule : TaggedFilterModuleBase<ColorWheelFilterData, ColorWheelFilterOutput, ColorWheelFilterDescriptor>
	{
		#region Public Methods

		/// <summary>
		/// Configures the filter.
		/// </summary>
		/// <returns>True if the filter was configured</returns>
		public override bool Setup()
		{
			// At this time the filter does not have a Setup dialog
			return false;
		}

		#endregion

		#region Public Properties

		public override bool HasSetup => false;

		/// <summary>
		/// Color wheel data associated with the function.
		/// </summary>
		public List<FixtureColorWheel> ColorWheelData
		{
			get { return Data.ColorWheelData; }
			set { Data.ColorWheelData = value; }
		}

		/// <summary>
		/// Flag which determines if RGB colors are converted into color wheel index commands.
		/// </summary>
		public bool ConvertRGBIntoIndexCommands
		{
			get { return Data.ConvertRGBIntoIndexCommands; }
			set { Data.ConvertRGBIntoIndexCommands = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override ColorWheelFilterOutput CreateOutputInternal()
		{
			// Create the color wheel tagged filter output
			return new ColorWheelFilterOutput(Data.Tag, Data.ColorWheelData, Data.ConvertRGBIntoIndexCommands);
		}

		#endregion
	}
}