using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS.Components.EntityConstructor;

internal class AtlasEntityConstructor { }

public abstract class AtlasEntityConstructor<T> : AtlasComponent<T>, IEntityConstructor<T> where T : class, IEntityConstructor
{
	#region Events
	public event Action<T, Construction, Construction> ConstructionChanged;

	event Action<IEntityConstructor, Construction, Construction> IEntityConstructor.ConstructionChanged
	{
		add => ConstructionChanged += value;
		remove => ConstructionChanged -= value;
	}
	#endregion

	private Construction construction = Construction.Deconstructed;
	private bool autoRemove = AtlasECS.AutoRemove;

	protected AtlasEntityConstructor() { }

	protected override void Disposing()
	{
		AutoRemove = AtlasECS.AutoRemove;
		base.Disposing();
	}

	protected override void AddingManager(IEntity entity, int index)
	{
		base.AddingManager(entity, index);
		Construction = Construction.Constructing;
	}

	protected override void RemovingManager(IEntity entity, int index)
	{
		Construction = Construction.Deconstructed;
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

	public Construction Construction
	{
		get => construction;
		private set
		{
			if(construction == value)
				return;
			var previous = construction;
			construction = value;
			ConstructionChanged?.Invoke(this as T, value, previous);

			if(value == Construction.Constructing)
				Construct();
			if(construction == Construction.Constructing)
			{
				Construction = Construction.Constructed;
				if(autoRemove)
					RemoveManagers();
			}
		}
	}

	protected abstract void Construct();
}