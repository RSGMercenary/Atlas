using Atlas.ECS.Components.EntityConstructor;

namespace Atlas.Tests.Testers.Components;

internal class TestEntityContructor : AtlasEntityConstructor<IEntityConstructor>
{
	internal TestEntityContructor(bool autoRemove = true)
	{
		AutoRemove = autoRemove;
	}

	protected override void Construct()
	{
		Manager.AddComponent<TestComponent>();
	}
}