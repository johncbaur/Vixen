using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Data associated with a tagged filter.
	/// </summary>
	public class TaggedFilterData : TaggedFilterDataBase
	{
		#region Public Methods

		/// <summary>
		/// Clones the filter data.
		/// </summary>
		/// <returns>Clone of the filter data</returns>
		public override IModuleDataModel Clone()
		{
			TaggedFilterData newInstance = (TaggedFilterData)MemberwiseClone();
			return newInstance;
		}

		#endregion
	}
}