using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Maintains a collection of fixture function objects.
	/// </summary>
	public class FixtureFunctionCollection : NotifyPropertyObservableCollection<IFixtureFunction>
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunctionCollection() : 
			base("FixtureFunctions")
		{
		}

		#endregion
	}
}
