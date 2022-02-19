using System;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Data.Value;
using Vixen.Module;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;

namespace VixenModules.Property.IntelligentFixture
{
	[DataContract]
	public class IntelligentFixtureData : ModuleDataModelBase, IDeserializationCallback
	{
		public IntelligentFixtureData()
		{
			FixtureSpecification = new FixtureSpecification();
			//InitializeX1Channels();
			//InitializeX7Channels(FixtureSpecification);
			//InitializeX7FunctionTypes(FixtureSpecification);
			//FixtureSpecification =
			//	FixtureSpecificationManager.Instance().Specifications.Single(spec => spec.Name == ADJHydroBeamX1Data.Name);
		}

		public override IModuleDataModel Clone()
		{
			IntelligentFixtureData clone = new IntelligentFixtureData();

			clone.FixtureSpecification = FixtureSpecification.CreateInstanceForClone();

			return clone;
		}

		[DataMember]
		public FixtureSpecification FixtureSpecification
		{
			get;
			set;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			if (FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "None") == null)
			{
				FixtureSpecification.AddFunctionType(
					"None",
					FixtureFunctionType.None,
					FunctionIdentity.Custom);
			}
           
			if (FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Pan") == null)
			{
				FixtureSpecification.AddFunctionType(
					"Pan",
					FixtureFunctionType.Range,
					FunctionIdentity.Pan);
			}
			else
            {
				FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Pan").FunctionIdentity = FunctionIdentity.Pan;
			}

			if (FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Tilt") == null)
			{
				FixtureSpecification.AddFunctionType(
					"Tilt",
					FixtureFunctionType.Range,
					FunctionIdentity.Tilt);
			}
			else
			{
				FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Tilt").FunctionIdentity = FunctionIdentity.Tilt;
			}

			if (FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Color Wheel") == null)
			{
				FixtureSpecification.AddFunctionType(
					"Color Wheel",
					FixtureFunctionType.ColorWheel,
					FunctionIdentity.Custom);
			}


			if (FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Dim") == null)
			{
				FixtureSpecification.AddFunctionType(
					"Dim",
					FixtureFunctionType.Range,
					FunctionIdentity.Dim);
			}
			else
			{
				FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Dim").FunctionIdentity = FunctionIdentity.Dim;
			}

			if (FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Zoom") == null)
			{
				FixtureSpecification.AddFunctionType(
					"Zoom",
					FixtureFunctionType.Range,
					FunctionIdentity.Zoom);
			}
			else
			{
				FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Zoom").FunctionIdentity = FunctionIdentity.Zoom;
			}

			/*
			if (FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Color") == null)
			{
				FixtureSpecification.AddFunctionType(
					"Color",
					FixtureFunctionType.Range,
					FunctionIdentity.Zoom);
			}
			else
			{
				FixtureSpecification.FunctionDefinitions.FirstOrDefault(function => function.Name == "Zoom").FunctionIdentity = FunctionIdentity.Zoom;
			}
			*/
		}	
	}
}