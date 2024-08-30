using Atlas.ECS.Components.Component;

namespace Atlas.Tests.Testers.Components;

interface ITestComponent : IComponent<ITestComponent>
{
}