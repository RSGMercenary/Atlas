using Atlas.ECS.Components.Component;

namespace Atlas.Tests.Testers.Components;

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

#pragma warning disable 0169 // Remove unused private members
	private TestComponent TestPrivateField1;
#pragma warning restore 0169 // Remove unused private members

#pragma warning disable 0169 // Remove unused private members
	private TestComponent TestPrivateField2;
#pragma warning restore 0169 // Remove unused private members
}