using Atlas.ECS.Families;
using Atlas.Tests.ECS.Components.Components;
using System.Diagnostics.CodeAnalysis;

namespace Atlas.Tests.ECS.Families.Families;

[ExcludeFromCodeCoverage]
public class TestFamilyMember : AtlasFamilyMember
{
    public TestComponent Component { get; }
}