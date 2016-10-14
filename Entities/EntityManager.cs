using Atlas.Components;
using Atlas.LinkList;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Entities
{
	sealed class EntityManager:Component, IEntityManager
	{
		private static EntityManager instance;

		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private Dictionary<string, IEntity> entityGlobalNames = new Dictionary<string, IEntity>();
		private Signal<IEntityManager, IEntity> entityAdded = new Signal<IEntityManager, IEntity>();
		private Signal<IEntityManager, IEntity> entityRemoved = new Signal<IEntityManager, IEntity>();

		private EntityManager() : base(false)
		{

		}

		public static EntityManager Instance
		{
			get
			{
				if(instance == null)
					instance = new EntityManager();
				return instance;
			}
		}

		override protected void AddingComponentManager(IEntity entity)
		{
			base.AddingComponentManager(entity);
			AddEntity(entity);
		}

		override protected void RemovingComponentManager(IEntity entity)
		{
			RemoveEntity(entity);
			base.RemovingComponentManager(entity);
		}

		public bool HasEntity(string globalName)
		{
			return !string.IsNullOrWhiteSpace(globalName) && !entityGlobalNames.ContainsKey(globalName);
		}

		public bool HasEntity(IEntity entity)
		{
			return entity != null && entityGlobalNames.ContainsKey(entity.GlobalName) && entityGlobalNames[entity.GlobalName] == entity;
		}

		public new Signal<IEntityManager, IEntity> EntityAdded
		{
			get
			{
				return entityAdded;
			}
		}

		public new Signal<IEntityManager, IEntity> EntityRemoved
		{
			get
			{
				return entityRemoved;
			}
		}

		public new bool SetEntityIndex(IEntity entity, int index)
		{
			return entities.SetIndex(entity, index);
		}

		public IEntity GetEntity(string globalName)
		{
			return entityGlobalNames.ContainsKey(globalName) ? entityGlobalNames[globalName] : null;
		}

		public new IReadOnlyLinkList<IEntity> Entities
		{
			get
			{
				return entities;
			}
		}

		private void AddEntity(Entity entity)
		{
			if(entityGlobalNames.ContainsKey(entity.GlobalName) && entityGlobalNames[entity.GlobalName] != entity)
			{
				entity.GlobalName = Guid.NewGuid().ToString("N");
			}
			if(!entityGlobalNames.ContainsKey(entity.GlobalName))
			{
				entityGlobalNames.Add(entity.GlobalName, entity);
				entities.Add(entity);

				entity.ChildAdded.Add(ChildAdded, int.MinValue);
				entity.ParentChanged.Add(ParentChanged, int.MinValue);
				entity.GlobalNameChanged.Add(UniqueNameChanged, int.MinValue);

				entity.EntityManager = this;

				entityAdded.Dispatch(this, entity);

				for(ILinkListNode<IEntity> current = entity.Children.First; current != null; current = current.Next)
				{
					AddEntity(current.Value);
				}
			}
		}

		private void RemoveEntity(Entity entity)
		{
			for(ILinkListNode<IEntity> current = entity.Children.First; current != null; current = current.Next)
			{
				RemoveEntity(current.Value);
			}

			entityRemoved.Dispatch(this, entity);

			entityGlobalNames.Remove(entity.GlobalName);
			entities.Remove(entity);

			entity.ChildAdded.Remove(ChildAdded);
			entity.ParentChanged.Remove(ParentChanged);
			entity.GlobalNameChanged.Remove(UniqueNameChanged);

			entity.EntityManager = null;
		}

		private void ChildAdded(IEntity parent, IEntity child, int index)
		{
			AddEntity(child);
		}

		private void ParentChanged(IEntity child, IEntity next, IEntity previous)
		{
			if(next == null)
			{
				RemoveEntity(child);
			}
		}

		private void UniqueNameChanged(IEntity entity, string next, string previous)
		{
			entityGlobalNames.Remove(previous);
			entityGlobalNames.Add(next, entity);
		}
	}
}