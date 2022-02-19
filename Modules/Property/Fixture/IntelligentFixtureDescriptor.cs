using System;
using Vixen.Module.Property;

namespace VixenModules.Property.IntelligentFixture
{
	public class ZoomDescriptor : PropertyModuleDescriptorBase
	{
		public static Guid _typeId = new Guid("{FDD74C85-1779-4521-B559-0DD5EA71054E}");

		public override string TypeName => "Intelligent Fixture";

		public override Guid TypeId => _typeId;

		public override Type ModuleClass => typeof (IntelligentFixtureModule);

		public override string Author => "Vixen Team";

		public override string Description => "Provides ability to specify capabilities of an intelligent fixture";

		public override string Version => "1.0";

		public override Type ModuleDataClass => typeof (IntelligentFixtureData);
	}
}