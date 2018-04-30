using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Messages;
using System;

namespace Atlas.Engine.Systems
{
	public abstract class AtlasFamilySystem<TFamilyType> : AtlasSystem
	{
		private IFamily family;
		private UpdatePhase updateMode = UpdatePhase.Update;
		private Action<double, IEntity> entityUpdate;
		private Action<IFamily, IEntity> entityAdded;
		private Action<IFamily, IEntity> entityRemoved;
		private bool updateSleepingEntities = false;
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

		protected void Initialize(Action<double, IEntity> entityUpdate, bool updateSleepingEntities, Action<IFamily, IEntity> entityAdded, Action<IFamily, IEntity> entityRemoved)
		{
			if(isInitialized)
				return;
			isInitialized = true;
			this.updateSleepingEntities = updateSleepingEntities;
			this.entityUpdate = entityUpdate;
			this.entityAdded = entityAdded;
			this.entityRemoved = entityRemoved;
		}

		override protected void Destroying()
		{
			family = null;
			entityUpdate = null;
			entityAdded = null;
			entityRemoved = null;
			base.Destroying();
		}

		public UpdatePhase UpdateMode
		{
			get { return updateMode; }
			set { updateMode = value; }
		}

		override protected void Updating(double deltaTime)
		{
			if(updateMode == UpdatePhase.Update)
				FamilyUpdate(deltaTime);
		}

		protected override void FixedUpdating(double deltaTime)
		{
			if(updateMode == UpdatePhase.FixedUpdate)
				FamilyUpdate(deltaTime);
		}

		private void FamilyUpdate(double deltaTime)
		{
			if(entityUpdate == null)
				return;
			if(family == null)
				return;
			foreach(IEntity entity in family.Entities)
			{
				if(updateSleepingEntities || !entity.IsSleeping)
					entityUpdate(deltaTime, entity);
			}
		}

		public bool UpdateSleepingEntities
		{
			get
			{
				return updateSleepingEntities;
			}
		}

		override protected void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			family = engine.AddFamily<TFamilyType>();
			if(entityAdded != null)
			{
				family.AddListener<IFamilyEntityAddMessage>(EntityAdded);
				foreach(IEntity entity in family.Entities)
				{
					entityAdded(family, entity);
				}
			}
			if(entityRemoved != null)
			{
				family.AddListener<IFamilyEntityRemoveMessage>(EntityRemoved);
			}
		}

		override protected void RemovingEngine(IEngine engine)
		{
			if(entityAdded != null)
			{
				family.RemoveListener<IFamilyEntityAddMessage>(EntityAdded);
			}
			if(entityRemoved != null)
			{
				family.RemoveListener<IFamilyEntityRemoveMessage>(EntityRemoved);
				foreach(IEntity entity in family.Entities)
				{
					entityRemoved(family, entity);
				}
			}
			engine.RemoveFamily<TFamilyType>();
			base.RemovingEngine(engine);
		}

		private void EntityAdded(IFamilyEntityAddMessage message)
		{
			entityAdded(message.Messenger, message.Value);
		}

		private void EntityRemoved(IFamilyEntityRemoveMessage message)
		{
			entityRemoved(message.Messenger, message.Value);
		}
	}
}