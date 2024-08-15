using Atlas.ECS.Components.Component;

namespace Atlas.Tests.ECS.Components.Components;

interface ITestComponent : IComponent<ITestComponent>
{
}