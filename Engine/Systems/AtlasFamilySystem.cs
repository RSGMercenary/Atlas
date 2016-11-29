using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.LinkList;
using System;

namespace Atlas.Engine.Systems
{
	abstract class AtlasFamilySystem<TFamilyType>:AtlasSystem
	{
		private IFamily family;
		private Action<IEntity> entityUpdate;
		private Action<IFamily, IEntity> entityAdded;
		private Action<IFamily, IEntity> entityRemoved;
		bool isInitialized = false;

		public AtlasFamilySystem()
		{

		}

		protected void Initialize(Action<IEntity> entityUpdate = null, Action<IFamily, IEntity> entityAdded = null, Action<IFamily, IEntity> entityRemoved = null)
		{
			if(isInitialized)
				return;
			isInitialized = true;
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

		override protected void Updating()
		{
			if(entityUpdate == null)
				return;
			ILinkListNode<IEntity> current = family.Entities.First;
			while(current != null)
			{
				entityUpdate(current.Value);
				current = current.Next;
			}
		}

		public IFamily Family
		{
			get
			{
				return family;
			}
		}

		override protected void AddingEngine(IEngineManager engine)
		{
			base.AddingEngine(engine);
			family = engine.AddFamily<TFamilyType>();
			if(entityAdded != null)
			{
				family.EntityAdded.Add(entityAdded);
				ILinkListNode<IEntity> current = family.Entities.First;
				while(current != null)
				{
					entityAdded(family, current.Value);
					current = current.Next;
				}
			}
			if(entityRemoved != null)
			{
				family.EntityRemoved.Add(entityRemoved);
			}
		}

		override protected void RemovingEngine(IEngineManager engine)
		{
			if(entityAdded != null)
			{
				family.EntityAdded.Remove(entityAdded);
			}
			if(entityRemoved != null)
			{
				family.EntityRemoved.Remove(entityRemoved);
				ILinkListNode<IEntity> current = family.Entities.First;
				while(current != null)
				{
					entityRemoved(family, current.Value);
					current = current.Next;
				}
			}
			engine.RemoveFamily<TFamilyType>();
			base.RemovingEngine(engine);
		}
	}
}