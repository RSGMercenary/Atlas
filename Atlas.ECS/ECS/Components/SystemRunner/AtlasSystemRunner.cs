using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.ECS.Components.SystemRunner;

public class AtlasSystemRunner : AtlasComponent<ISystemRunner>, ISystemRunner
{
	public event Action<ISystemRunner, Type> Added;
	public event Action<ISystemRunner, Type> Removed;

	private readonly HashSet<Type> types = new();

	public AtlasSystemRunner() { }

	public AtlasSystemRunner(params Type[] types) : this(types as IEnumerable<Type>) { }

	public AtlasSystemRunner(IEnumerable<Type> types)
	{
		foreach(var type in types)
			AddSystem(type);
	}

	protected override void Disposing()
	{
		RemoveSystems();
		base.Disposing();
	}

	#region Engine/Systems
	protected override void AddingManager(IEntity entity, int index)
	{
		base.AddingManager(entity, index);
		UpdateSystems(entity.Engine, true);
		entity.EngineChanged += UpdateSystems;
	}

	protected override void RemovingManager(IEntity entity, int index)
	{
		entity.EngineChanged -= UpdateSystems;
		UpdateSystems(entity.Engine, false);
		base.RemovingManager(entity, index);
	}

	private void UpdateSystems(IEntity entity, IEngine current, IEngine previous)
	{
		UpdateSystems(previous, false);
		UpdateSystems(current, true);
	}

	private void UpdateSystems(IEngine engine, bool add)
	{
		if(engine == null)
			return;
		foreach(var type in types)
		{
			if(add)
				engine.Systems.Add(type);
			else
				engine.Systems.Remove(type);
		}
	}
	#endregion

	#region Get
	public IReadOnlySet<Type> Systems => types;
	#endregion

	#region Has
	public bool HasSystem<TSystem>() where TSystem : class, ISystem => HasSystem(typeof(TSystem));

	public bool HasSystem(Type type) => types.Contains(type);
	#endregion

	#region Add
	public bool AddSystem<TSystem>() where TSystem : class, ISystem => AddSystem(typeof(TSystem));

	public bool AddSystem(Type type)
	{
		if(!typeof(ISystem).IsAssignableFrom(type))
			return false;
		if(types.Contains(type))
			return false;
		types.Add(type);
		Added?.Invoke(this, type);
		Manager?.Engine?.Systems.Add(type);
		return true;
	}
	#endregion

	#region Remove
	public bool RemoveSystem<TSystem>() where TSystem : class, ISystem => RemoveSystem(typeof(TSystem));

	public bool RemoveSystem(Type type)
	{
		if(!typeof(ISystem).IsAssignableFrom(type))
			return false;
		if(!types.Contains(type))
			return false;
		types.Remove(type);
		Removed?.Invoke(this, type);
		Manager?.Engine?.Systems.Remove(type);
		return true;
	}

	public bool RemoveSystems()
	{
		if(types.Count <= 0)
			return false;
		foreach(var type in types.ToArray())
			RemoveSystem(type);
		return true;
	}
	#endregion
}