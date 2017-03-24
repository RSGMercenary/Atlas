using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.Engine.Families
{
	sealed class AtlasFamily : EngineObject<IFamily>, IFamily
	{
		private Type familyType;
		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private HashSet<IEntity> entitySet = new HashSet<IEntity>();
		private List<Type> components = new List<Type>();
		private HashSet<Type> componentsSet = new HashSet<Type>();

		private Signal<IFamily, IEntity> entityAdded = new Signal<IFamily, IEntity>();
		private Signal<IFamily, IEntity> entityRemoved = new Signal<IFamily, IEntity>();

		public AtlasFamily()
		{

		}

		public IReadOnlyLinkList<IEntity> Entities { get { return entities; } }

		public ISignal<IFamily, IEntity> EntityAdded { get { return entityAdded; } }
		public ISignal<IFamily, IEntity> EntityRemoved { get { return entityRemoved; } }

		sealed public override bool Destroy()
		{
			if(State != EngineObjectState.Constructed)
				return false;
			Engine = null;
			if(Engine == null)
				return base.Destroy();
			return false;
		}

		protected override void Destroying()
		{
			SetFamilyType(null);
			entityAdded.Dispose();
			entityRemoved.Dispose();
			base.Destroying();
		}

		sealed override public IEngine Engine
		{
			get
			{
				return base.Engine;
			}
			set
			{
				if(value != null)
				{
					if(Engine == null && value.HasFamily(this))
					{
						base.Engine = value;
					}
				}
				else
				{
					if(Engine != null && !Engine.HasFamily(this))
					{
						base.Engine = value;
					}
				}
			}
		}

		public Type FamilyType
		{
			get
			{
				return familyType;
			}
			set
			{
				if(familyType != null)
					return;
				SetFamilyType(value);
			}
		}

		private void SetFamilyType(Type value)
		{
			if(familyType == value)
				return;
			familyType = value;
			components.Clear();
			componentsSet.Clear();
			if(familyType != null)
			{
				foreach(FieldInfo info in familyType.GetFields())
				{
					Type component = info.FieldType;
					components.Add(component);
					componentsSet.Add(component);
				}
			}
		}

		public void AddEntity(IEntity entity)
		{
			Add(entity);
		}

		public void RemoveEntity(IEntity entity)
		{
			Remove(entity);
		}

		public void AddEntity(IEntity entity, Type componentType)
		{
			if(componentsSet.Contains(componentType))
			{
				Add(entity);
			}
		}

		public void RemoveEntity(IEntity entity, Type componentType)
		{
			if(componentsSet.Contains(componentType))
			{
				Remove(entity);
			}
		}

		private void Add(IEntity entity)
		{
			if(entitySet.Contains(entity))
				return;
			foreach(Type type in components)
			{
				if(!entity.HasComponent(type))
					return;
			}
			entities.Add(entity);
			entitySet.Add(entity);
			entityAdded.Dispatch(this, entity);
		}

		private void Remove(IEntity entity)
		{
			if(!entitySet.Contains(entity))
				return;
			entities.Remove(entity);
			entitySet.Remove(entity);
			entityRemoved.Dispatch(this, entity);
		}
	}
}
