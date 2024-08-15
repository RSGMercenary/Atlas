using Atlas.ECS.Components.Component;

namespace Atlas.Tests.ECS.Components.Components;

class TestComponent : AtlasComponent<ITestComponent>, ITestComponent
{
	public bool TestUpdate = false;
	public bool TestDispose = false;

	public TestComponent() : base() { }

	public TestComponent(bool isShareable) : base(isShareable) { }

	public override void Dispose()
	{
		base.Dispose();
		TestDispose = true;
	}
}