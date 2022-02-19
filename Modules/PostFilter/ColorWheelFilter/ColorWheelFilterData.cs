using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Fixture;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.ColorWheelFilter
{
	/// <summary>
	/// Maintains the color wheel filter data.
	/// </summary>
	public class ColorWheelFilterData : ModuleDataModelBase, ITaggedFilterData
	{
		#region Public Methods

		/// <summary>
		/// Clones the color wheel filter data model.
		/// </summary>
		/// <returns>A copy of the color wheel data model</returns>
		public override IModuleDataModel Clone()
		{
			// Create a new color wheel filter data instance
			ColorWheelFilterData newInstance = new ColorWheelFilterData();
			
			// Copy the color wheel tag
			newInstance.Tag = Tag;
			
			// Loop over the color wheel items
			foreach (FixtureColorWheel colorWheelItem in ColorWheelData)
			{
				// Clone the color wheel item
				newInstance.ColorWheelData.Add(colorWheelItem.CreateInstanceForClone());
			}

			// Copy whether to convert RGB colors into color wheel index commands
			newInstance.ConvertRGBIntoIndexCommands = ConvertRGBIntoIndexCommands;

			// Return the new color wheel filter data instance
			return newInstance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Color wheel data associated with the function.
		/// </summary>
		[DataMember]
		public List<FixtureColorWheel> ColorWheelData { get; set; }

		/// <summary>
		/// Tag associated with the color wheel function.
		/// </summary>
		[DataMember]
		public string Tag { get; set; }

		/// <summary>
		/// Flag which determines if RGB colors are converted into color wheel index commands.
		/// </summary>
		[DataMember]
		public bool ConvertRGBIntoIndexCommands { get; set; }

		#endregion
	}
}