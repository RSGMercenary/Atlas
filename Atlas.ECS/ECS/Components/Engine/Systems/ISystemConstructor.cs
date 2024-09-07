using Atlas.ECS.Systems;

namespace Atlas.ECS.Components.Engine.Systems;

public interface ISystemConstructor
{
	TSystem Construct<TSystem>() where TSystem : class, ISystem;
}