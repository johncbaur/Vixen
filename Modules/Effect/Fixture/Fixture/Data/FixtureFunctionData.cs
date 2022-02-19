using System;
using System.Runtime.Serialization;
using Vixen.Data.Value;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using ZedGraph;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Serialized data for a fixture range.
	/// </summary>
	[DataContract]
	public class FixtureFunctionData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureFunctionData()
		{
			FunctionIdentity = FunctionIdentity.Custom;
			FunctionName = String.Empty;
			Range = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 0.0 }));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a clone of the waveform data.
		/// </summary>		
		public FixtureFunctionData CreateInstanceForClone()
		{
			FixtureFunctionData result = new FixtureFunctionData
			{
				FunctionName = FunctionName,
				FunctionIdentity = FunctionIdentity,
				Range = new Curve(Range),
				FunctionType = FunctionType,
				IndexValue = IndexValue,
				ColorIndexValue = ColorIndexValue,
				Enable = Enable,
			};

			return result;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public bool Enable { get; set; }

		[DataMember]
		public Curve Range { get; set; }

		[DataMember]
		public string FunctionName { get; set; }

		[DataMember]
		public FunctionIdentity FunctionIdentity { get; set; }

		[DataMember]
		public string IndexValue { get; set; }

		[DataMember]
		public string ColorIndexValue { get; set; }

		[DataMember]
		public FixtureFunctionType FunctionType { get; set; }
		
		#endregion
	}
}
