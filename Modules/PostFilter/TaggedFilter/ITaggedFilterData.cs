namespace VixenModules.OutputFilter.TaggedFilter.Outputs
{
	/// <summary>
	/// Maintains tagged filter data.
	/// </summary>
	public interface ITaggedFilterData
	{
		/// <summary>
		/// Tag (function) associated with the filter.
		/// </summary>
		string Tag { get; set; }
	}
}
