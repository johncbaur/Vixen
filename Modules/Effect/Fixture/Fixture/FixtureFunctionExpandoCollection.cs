using System;
using System.Collections.Generic;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Maintains a collection of fixture function expando objects.
	/// </summary>
	public class FixtureFunctionExpandoCollection : NotifyPropertyObservableCollection<IFixtureFunctionExpando>,
		IDiscoverCollectionItemType
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunctionExpandoCollection() :
			base("FixtureFunctions")
		{
		}

		#endregion

		#region IDiscoverCollectionItemType

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public Type GetItemType()
		{
			// Return the concrete type of the object in the collection
			return typeof(FixtureFunctionExpando);	
		}

		#endregion
	}
}
