using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.ECS.Components.Builder;

public abstract class AtlasBuilder : AtlasComponent<IBuilder>, IBuilder
{
	private readonly Stack<Action> builders = new();
	private BuildState state = BuildState.Unbuilt;
	private bool autoRemove = true;

	protected override void AddingManager(IEntity entity, int index)
	{
		base.AddingManager(entity, index);
		BuildState = BuildState.Building;
	}

	protected override void RemovingManager(IEntity entity, int index)
	{
		BuildState = BuildState.Unbuilt;
		base.RemovingManager(entity, index);
	}

	public bool AutoRemove
	{
		get => autoRemove;
		set
		{
			if(autoRemove == value)
				return;
			autoRemove = value;
		}
	}

	public BuildState BuildState
	{
		get => state;
		private set
		{
			if(state == value)
				return;
			var previous = state;
			state = value;
			Message<IBuildStateMessage>(new BuildStateMessage(state, previous));
			if(value == BuildState.Building)
			{
				var type = GetType();
				var stop = typeof(AtlasBuilder);
				while(type != stop)
				{
					var info = type.GetMethod(nameof(Building), BindingFlags.NonPublic | BindingFlags.Instance);
					//If a class doesn't override the method, it inherits it.
					//This prevents method calls being duplicated.
					if(type == info.DeclaringType)
					{
						var pointer = info.MethodHandle.GetFunctionPointer();
						var builder = (Action)Activator.CreateInstance(typeof(Action), this, pointer);
						builders.Push(builder);
					}
					type = type.BaseType;
				}
				Built();
			}
		}
	}

	/// <summary>
	/// Overridden 'Building()' methods will be invoked
	/// sequentially from base classes to sub classes (similar to a constructor), instructing your
	/// subclass to begin building. Once your sub class is finished building, call Built()
	/// so the Builder may proceed to build the next subclass.
	/// </summary>
	protected void Built()
	{
		if(state != BuildState.Building)
			return;
		if(builders.Count > 0)
			builders.Pop().Invoke();
		else
		{
			//If we have no more builders to invoke,
			//then building is complete.
			BuildState = BuildState.Built;
			if(autoRemove)
				RemoveManagers();
		}
	}

	/// <summary>
	/// Override this method when you want a subclass of another builder to add more building AFTER the base class
	/// has finished. When overriding this method, NEVER call 'base.Building()'.
	/// </summary>
	protected abstract void Building();
}