using Atlas.Core.Collections.LinkList;
using Atlas.Core.Extensions;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Systems;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal sealed class SystemManager : ISystemManager
{
	#region Events
	public event Action<ISystemManager, ISystem, Type> Added;
	public event Action<ISystemManager, ISystem, Type> Removed;
	#endregion

	#region Fields
	private readonly LinkList<ISystem> fixedSystems = new();
	private readonly LinkList<ISystem> variableSystems = new();
	private readonly Dictionary<Type, ISystem> types = new();
	private readonly Dictionary<Type, int> references = new();
	#endregion

	public SystemManager(IEngine engine)
	{
		Engine = engine;
	}

	public IEngine Engine { get; }

	#region Create
	public ISystemCreator Creator { get; set; }

	private TSystem CreateSystem<TSystem>() where TSystem : class, ISystem => Creator != null ? Creator.Create<TSystem>() : SystemGetter.GetSystem<TSystem>();

	private T GenericInvoke<T>(Type type, string name)
	{
		if(!typeof(ISystem).IsAssignableFrom(type))
			AtlasThrower.NotAssignable(type, typeof(ISystem), nameof(type));
		return (T)GetType()
			.GetMethod(name, 1, [])
			.MakeGenericMethod(type)
			.Invoke(this, null);
	}
	#endregion

	#region Add
	public TSystem Add<TSystem>() where TSystem : class, ISystem
	{
		var type = typeof(TSystem);
		if(!types.TryGetValue(type, out TSystem system))
		{
			system = CreateSystem<TSystem>();

			system.PriorityChanged += PriorityChanged;
			system.TimeStepChanged += TimeStepChanged;

			Add(system);

			types.Add(type, system);
			references.Add(type, 0);
			system.Engine = Engine;

			Added?.Invoke(this, system, type);
		}
		++references[type];
		return (TSystem)types[type];
	}

	public ISystem Add(Type type) => GenericInvoke<ISystem>(type, nameof(Add));
	#endregion

	#region Remove
	public bool Remove<TSystem>() where TSystem : class, ISystem
	{
		var type = typeof(TSystem);
		if(!types.TryGetValue(type, out var system))
			return false;
		if(--references[type] > 0)
			return false;

		system.PriorityChanged -= PriorityChanged;
		system.TimeStepChanged -= TimeStepChanged;

		Remove(system);

		types.Remove(type);
		references.Remove(type);
		system.Engine = null;

		Removed?.Invoke(this, system, type);
		return true;
	}

	public bool Remove(Type type) => GenericInvoke<bool>(type, nameof(Remove));
	#endregion

	#region Get
	[JsonProperty]
	public IReadOnlyLinkList<ISystem> FixedSystems => fixedSystems;

	[JsonProperty]
	public IReadOnlyLinkList<ISystem> VariableSystems => variableSystems;

	public IReadOnlyDictionary<Type, ISystem> Types => types;

	public TSystem Get<TSystem>() where TSystem : class, ISystem => (TSystem)Get(typeof(TSystem));

	public ISystem Get(Type type) => types.TryGetValue(type, out var system) ? system : null;

	public ISystem Get(TimeStep timeStep, int index) => Get(timeStep)[index];
	#endregion

	#region Has
	public bool Has<TSystem>() where TSystem : class, ISystem => Has(typeof(TSystem));

	public bool Has(Type type) => Get(type) != null;

	public bool Has(ISystem system) => types.ContainsValue(system);
	#endregion

	#region Listeners
	private void TimeStepChanged(ISystem system, TimeStep current, TimeStep previous) => Set(system, current, previous);

	private void PriorityChanged(ISystem system, int current = -1, int previous = -1) => Set(system, system.TimeStep, system.TimeStep);
	#endregion

	#region Priority
	private void Set(ISystem system, TimeStep current, TimeStep previous)
	{
		Remove(system, previous);
		Add(system, current);
	}

	private void Remove(ISystem system, TimeStep? timeStep = null)
	{
		Get(timeStep ?? system.TimeStep)?.Remove(system);
	}

	private void Add(ISystem system, TimeStep? timeStep = null)
	{
		var systems = Get(timeStep ?? system.TimeStep);

		if(systems == null)
			return;

		var index = systems.Count - 1;
		for(var current = systems.Last; current != null; current = current.Previous)
		{
			if(current.Value.Priority <= system.Priority)
			{
				systems.Add(system, index);
				return;
			}
			--index;
		}
		systems.Add(system, 0);
	}

	private LinkList<ISystem> Get(TimeStep timeStep)
	{
		if(timeStep == TimeStep.Fixed)
			return fixedSystems;
		if(timeStep == TimeStep.Variable)
			return variableSystems;
		return null;
	}
	#endregion
}