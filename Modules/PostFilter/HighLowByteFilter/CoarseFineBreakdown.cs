using Vixen.Module;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Data associated with the breakdown filter.
	/// </summary>
	public class CoarseFineBreakdownData : ModuleDataModelBase
	{	
		#region IModuleDataModel

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public override IModuleDataModel Clone()
		{
			CoarseFineBreakdownData newInstance = (CoarseFineBreakdownData)MemberwiseClone();
			
			return newInstance;
		}

		#endregion
	}
}