using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Data.Value;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	[DataContract]
	public class FixtureData : EffectTypeModuleData
	{
		public FixtureData()
		{			
			FunctionData = new List<FixtureFunctionData>();
			FixtureFunctions = new List<App.Fixture.FixtureFunction>();

			// TODO: Is this code necessary?

			/*
			// 1st range is entirely default
			FunctionData.Add(new FixtureFunctionData());

			FixtureFunctionData tilt = new FixtureFunctionData();
			tilt.FunctionIdentity = FunctionIdentity.Tilt;
			tilt.Tag = "Tilt";
			FunctionData.Add(tilt);

			FixtureFunctionData zoom = new FixtureFunctionData();
			zoom.FunctionIdentity = FunctionIdentity.Zoom;
			zoom.Tag = "Zoom";
			FunctionData.Add(zoom);

			FixtureFunctionData userDefined = new FixtureFunctionData();
			userDefined.FunctionIdentity = FunctionIdentity.Custom;
			userDefined.Tag = "Prism Rotate";
			FunctionData.Add(userDefined);
			*/

			ShowLegend = true;
		}

		[DataMember]
		public List<App.Fixture.FixtureFunction> FixtureFunctions { get; set; }


		[DataMember]
		public List<FixtureFunctionData> FunctionData { get; set; }

		[DataMember]
		public bool ShowLegend { get; set; }

		#region Protected Methods

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			FixtureData result = new FixtureData
			{				
			};
			
			// Clone the waves
			for (int index = 0; index < FunctionData.Count; index++)
			{
				result.FunctionData[index] = (FixtureFunctionData)(FunctionData[index]).CreateInstanceForClone();
			}

			return result;
		}

		#endregion
	}
}