using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS.Components.Builder;

public abstract class AtlasBuilder : AtlasComponent<IBuilder<AtlasBuilder>>, IBuilder<AtlasBuilder>
{
	public event Action<AtlasBuilder, BuildState, BuildState> BuildStateChanged;

	private BuildState state = BuildState.Unbuilt;
	private bool autoRemove = true;

	protected AtlasBuilder() { }

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
			BuildStateChanged?.Invoke(this, value, previous);
			if(value == BuildState.Building)
			{
				Building();
				BuildState = BuildState.Built;
				if(autoRemove)
					RemoveManagers();
			}
		}
	}

	protected abstract void Building();
}