using System.Collections.Generic;
using System.ComponentModel;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Maintains a tagged filter module.
	/// </summary>
	public class TaggedFilterModule : TaggedFilterModuleBase<TaggedFilterData, TaggedFilterOutput, TaggedFilterDescriptor>
	{
		#region Public Properties

		public override bool HasSetup => false;

		#endregion

		public override bool Setup()
		{
			//using (FixtureBreakdownSetup setup = new FixtureBreakdownSetup(_data)) {
			//	if (setup.ShowDialog() == DialogResult.OK) {					
			//		_data.FixtureName = setup.SelectedFixtureSpecification;
			//		_CreateOutputs(_data.FixtureName);
			//		return true;
			//	}
			//}
			return true;
		}

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override TaggedFilterOutput CreateOutputInternal()
		{
			// Create the tagged filter output
			return new TaggedFilterOutput(Data.Tag);
		}

		#endregion
	}
}