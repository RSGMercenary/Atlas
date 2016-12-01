using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Systems
{
	abstract class AtlasFamilySystem<TFamilyType>:AtlasSystem
	{
		private IFamily family;
		private Action<double, IEntity> entityUpdate;
		private Action<IFamily, IEntity> entityAdded;
		private Action<IFamily, IEntity> entityRemoved;
		private bool updateEntitiesSleeping = false;
		private bool isInitialized = false;

		public AtlasFamilySystem()
		{

		}

		protected void Initialize(Action<double, IEntity> entityUpdate, bool updateEntitiesSleeping = false)
		{
			Initialize(entityUpdate, updateEntitiesSleeping, null, null);
		}

		protected void Initialize(Action<IFamily, IEntity> entityAdded = null, Action<IFamily, IEntity> entityRemoved = null)
		{
			Initialize(null, false, entityAdded, entityRemoved);
		}

		protected void Initialize(Action<double, IEntity> entityUpdate, bool updateEntitiesSleeping, Action<IFamily, IEntity> entityAdded, Action<IFamily, IEntity> entityRemoved)
		{
			if(isInitialized)
				return;
			isInitialized = true;
			this.updateEntitiesSleeping = updateEntitiesSleeping;
			this.entityUpdate = entityUpdate;
			this.entityAdded = entityAdded;
			this.entityRemoved = entityRemoved;
		}

		protected override void Disposing()
		{
			family = null;
			entityUpdate = null;
			entityAdded = null;
			entityRemoved = null;
			base.Disposing();
		}

		public IFamily Family
		{
			get
			{
				return family;
			}
		}

		public Action<double, IEntity> EntityUpdate
		{
			get
			{
				return entityUpdate;
			}
		}

		public Action<IFamily, IEntity> EntityAdded
		{
			get
			{
				return entityAdded;
			}
		}

		public Action<IFamily, IEntity> EntityRemoved
		{
			get
			{
				return entityRemoved;
			}
		}

		public bool UpdateEntitiesSleeping
		{
			get
			{
				return updateEntitiesSleeping;
			}
		}

		override protected void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			family = engine.AddFamily<TFamilyType>();
			if(entityAdded != null)
			{
				family.EntityAdded.Add(entityAdded);
				for(var current = family.Entities.First; current != null; current = current.Next)
				{
					entityAdded(family, current.Value);
				}
			}
			if(entityRemoved != null)
			{
				family.EntityRemoved.Add(entityRemoved);
			}
		}

		override protected void RemovingEngine(IEngine engine)
		{
			if(entityAdded != null)
			{
				family.EntityAdded.Remove(entityAdded);
			}
			if(entityRemoved != null)
			{
				family.EntityRemoved.Remove(entityRemoved);
				for(var current = family.Entities.First; current != null; current = current.Next)
				{
					entityRemoved(family, current.Value);
				}
			}
			engine.RemoveFamily<TFamilyType>();
			base.RemovingEngine(engine);
		}
	}
}