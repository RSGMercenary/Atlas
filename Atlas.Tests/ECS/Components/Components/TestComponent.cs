using Atlas.ECS.Components.Component;
using System.Diagnostics.CodeAnalysis;

namespace Atlas.Tests.ECS.Components.Components;

[ExcludeFromCodeCoverage]
public class TestComponent : AtlasComponent<ITestComponent>, ITestComponent
{
    public bool TestDispose = false;

    public TestComponent() : base() { }

    public TestComponent(bool isShareable) : base(isShareable) { }

    public override void Dispose()
    {
        base.Dispose();
        TestDispose = true;
    }
}