using Atlas.ECS.Components.Engine;
using Atlas.ECS.Systems;
using System.Diagnostics.CodeAnalysis;

namespace Atlas.Tests.ECS.Systems.Systems;

[ExcludeFromCodeCoverage]
public class TestMultipleSystem1 : AtlasSystem, ITestMultipleSystem
{
    protected override void AddingEngine(IEngine engine)
    {

    }

    protected override void RemovingEngine(IEngine engine)
    {

    }
}