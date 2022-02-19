using System.Linq;
using System.Windows.Forms;
using Vixen.Data.Value;
using Vixen.Module;
using Vixen.Module.Property;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.Editor.FixturePropertyEditor.Views;

namespace VixenModules.Property.IntelligentFixture
{
	public class IntelligentFixtureModule : PropertyModuleInstanceBase
	{
		private IntelligentFixtureData _data;

		public override void SetDefaultValues()
		{
			//_data.ZoomMinimum = 6;
			//_data.ZoomMaximum = 40;

			_data.FixtureSpecification.FunctionDefinitions.Clear();

			_data.FixtureSpecification.AddFunctionType(
							"Pan",
							FixtureFunctionType.Range,
							FunctionIdentity.Pan);

			_data.FixtureSpecification.AddFunctionType(
				"Tilt",
				FixtureFunctionType.Range,
				FunctionIdentity.Tilt);

			FixtureFunction colorWheel = _data.FixtureSpecification.AddFunctionType(
				"Color Wheel",
				FixtureFunctionType.ColorWheel,
				FunctionIdentity.Custom);
			//InitializeColorWheel(colorWheel);

			_data.FixtureSpecification.AddFunctionType(
				"Dim",
				FixtureFunctionType.Range,
				FunctionIdentity.Dim);

			_data.FixtureSpecification.AddFunctionType(
				"Color",
				FixtureFunctionType.RGBWColor,
				FunctionIdentity.Custom);

			_data.FixtureSpecification.AddFunctionType(
				"Zoom",
				FixtureFunctionType.Range,
				FunctionIdentity.Zoom);

			_data.FixtureSpecification.AddFunctionType(
				"Shutter",
				FixtureFunctionType.Indexed,
				FunctionIdentity.Shutter);

			_data.FixtureSpecification.AddFunctionType(
				"None",
				FixtureFunctionType.None,
				FunctionIdentity.Custom);
		}

		public override bool HasSetup => true;

		public override bool Setup()
		{
			FixturePropertyEditorWindowView view = new FixturePropertyEditorWindowView(_data.FixtureSpecification);
			bool? result = view.ShowDialog();

			if (result.Value)
			{
				_data.FixtureSpecification = view.GetFixtureSpecification();
			}
			
			return true;
			/*
			using (ZoomSetup setupForm = new ZoomSetup(_data.ZoomMinimum, _data.ZoomMaximum)) {
				if (setupForm.ShowDialog() == DialogResult.OK)
				{

					_data.ZoomMinimum = setupForm.MinZoom;
					_data.ZoomMaximum = setupForm.MaxZoom;

					return true;
				}
				return false;
			}
			*/
		}

		//public override IModuleDataModel StaticModuleData
		//{
		//	get => _data;
		//	set => _data = value as IntelligentFixtureData;
		//}

		public override IModuleDataModel ModuleData
		{
			get => _data;
			set
			{
				_data = value as IntelligentFixtureData;

				if (!string.IsNullOrEmpty(_data.FixtureSpecification.Name))
				{
					IFixtureSpecificationManager fixtureSpecificationManager = FixtureSpecificationManager.Instance();
					FixtureSpecification specification = fixtureSpecificationManager.FixtureSpecifications.SingleOrDefault(item => item.Name == _data.FixtureSpecification.Name);

					if (specification != null)
					{
						// THIS IS STOMPING THE XML FILES!
						//fixtureSpecificationManager.FixtureSpecifications.Remove(specification);						
					}
					else
					{
						fixtureSpecificationManager.FixtureSpecifications.Add(_data.FixtureSpecification);
					}
				}
			}
		}

		public FixtureSpecification GetFixtureSpecification()
		{
			return _data.FixtureSpecification;
		}


	}
}