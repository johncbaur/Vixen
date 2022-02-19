using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.DimmingFilter.Outputs;
using VixenModules.OutputFilter.FixtureBreakdown;
using VixenModules.OutputFilter.TaggedFilter;
using VixenModules.OutputFilter.TaggedFilter.Outputs;


namespace VixenModules.OutputFilter.DimmingFilter
{
	/// <summary>
	/// Maintains a dimming filter module.
	/// </summary>
	public class DimmingFilterModule : TaggedFilterModuleBase<DimmingFilterData, DimmingFilterOutput, DimmingFilterDescriptor>
	{
		#region Public Methods

		/// <summary>
		/// Configures the filter.
		/// </summary>
		/// <returns>True if the filter was configured</returns>
		public override bool Setup()
		{
			bool okSelected = false;

			using (DimmingFilterSetup setup = new DimmingFilterSetup(Data))
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					CreateOutput();
					okSelected = true;
				}
			}

			return okSelected;
		}

		#endregion

		#region Public Properties

		public override bool HasSetup => true;

		/// <summary>
		/// Flag which determines if RGB colors are converted into dimming intents.
		/// </summary>
		public bool ConvertRGBIntoDimmingIntents
		{
			get { return Data.ConvertRGBIntoDimmingIntents; }
			set { Data.ConvertRGBIntoDimmingIntents = value; }
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override DimmingFilterOutput CreateOutputInternal()
		{
			// Create the tagged filter output
			return new DimmingFilterOutput(Data.Tag, Data.ConvertRGBIntoDimmingIntents);
		}

		#endregion
	}
}