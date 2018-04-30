using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Messages;

namespace Atlas.Engine.Systems
{
	public abstract class AtlasFamilySystem<TFamilyType> : AtlasSystem, IFamilySystem
	{
		private IFamily family;
		private UpdatePhase updateMode = UpdatePhase.Update;
		private bool updateSleepingEntities = false;

		public AtlasFamilySystem()
		{

		}

		override protected void Destroying()
		{
			family = null;
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
			if(family == null)
				return;
			var updateSleepingEntities = UpdateSleepingEntities;
			foreach(var entity in family.Entities)
			{
				if(updateSleepingEntities || !entity.IsSleeping)
					EntityUpdate(deltaTime, entity);
			}
		}

		virtual protected void EntityUpdate(double deltaTime, IEntity entity)
		{

		}

		virtual protected void EntityAdded(IFamily family, IEntity entity)
		{

		}

		virtual protected void EntityRemoved(IFamily family, IEntity entity)
		{

		}

		public bool UpdateSleepingEntities
		{
			get { return updateSleepingEntities; }
			set { updateSleepingEntities = value; }
		}

		override protected void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			family = engine.AddFamily<TFamilyType>();
			family.AddListener<IFamilyEntityAddMessage>(EntityAdded);
			family.AddListener<IFamilyEntityRemoveMessage>(EntityRemoved);
			foreach(IEntity entity in family.Entities)
			{
				EntityAdded(family, entity);
			}
		}

		override protected void RemovingEngine(IEngine engine)
		{
			family.RemoveListener<IFamilyEntityAddMessage>(EntityAdded);
			family.RemoveListener<IFamilyEntityRemoveMessage>(EntityRemoved);
			foreach(IEntity entity in family.Entities)
			{
				EntityRemoved(family, entity);
			}
			engine.RemoveFamily<TFamilyType>();
			family = null; //TO-DO Pretty sure this is safe.
			base.RemovingEngine(engine);
		}

		private void EntityAdded(IFamilyEntityAddMessage message)
		{
			EntityAdded(message.Messenger, message.Value);
		}

		private void EntityRemoved(IFamilyEntityRemoveMessage message)
		{
			EntityRemoved(message.Messenger, message.Value);
		}
	}
}