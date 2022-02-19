using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Fixture
{
	public class FixtureDescriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{E97BE031-05CF-46F2-ADD9-7E5209FEAB2E}");

		public override string TypeName => "Fixture";

		public override Guid TypeId => _typeId;

		public override Type ModuleClass => typeof (Fixture);

		public override Type ModuleDataClass => typeof (FixtureData);

		public override string Author => "Vixen Team";

		public override string Description => "Controls settings of an intelligent fixture";

		public override string Version => "1.0";

		public override string EffectName => "Fixture";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.Device;
	}
}