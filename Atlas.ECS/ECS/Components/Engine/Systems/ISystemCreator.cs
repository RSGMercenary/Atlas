using Atlas.ECS.Systems;

namespace Atlas.ECS.Components.Engine.Systems;

public interface ISystemCreator
{
	TSystem Create<TSystem>() where TSystem : class, ISystem;
}