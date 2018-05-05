using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using Atlas.Framework.Collections.EngineList;
using Atlas.Framework.Messages;
using Atlas.Framework.Objects;
using Atlas.Framework.Pools;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.ECS.Families
{
	sealed class AtlasFamily<TFamilyMember> : EngineObject, IFamily<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		private EngineList<TFamilyMember> members = new EngineList<TFamilyMember>();
		private Dictionary<IEntity, TFamilyMember> entities = new Dictionary<IEntity, TFamilyMember>();
		private Dictionary<Type, string> components = new Dictionary<Type, string>();
		private Stack<TFamilyMember> removed = new Stack<TFamilyMember>();
		private Pool<TFamilyMember> pooled = new Pool<TFamilyMember>(() => new TFamilyMember());

		public AtlasFamily()
		{
			foreach(var property in typeof(TFamilyMember).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if(property.Name != "Entity")
				{
					components.Add(property.PropertyType, property.Name);
				}
			}
		}

		public IReadOnlyEngineList<TFamilyMember> Members { get { return members; } }

		IReadOnlyEngineList<IFamilyMember> IReadOnlyFamily.Members
		{
			get { return members as IReadOnlyEngineList<IFamilyMember>; }
		}

		sealed public override void Dispose()
		{
			if(State != ObjectState.Initialized)
				return;
			//Can't destroy Family mid-update.
			if(Engine == null || Engine.UpdateState != UpdatePhase.None)
				return;
			Engine = null;
			if(Engine == null)
				base.Dispose();
		}

		protected override void Disposing(bool finalizer)
		{
			//TO-DO
			//Do some clean please! 
			base.Disposing(finalizer);
		}

		sealed override public IEngine Engine
		{
			get { return base.Engine; }
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
			if(components.ContainsKey(componentType))
			{
				Add(entity);
			}
		}

		public void RemoveEntity(IEntity entity, Type componentType)
		{
			if(components.ContainsKey(componentType))
			{
				Remove(entity);
			}
		}

		private void Add(IEntity entity)
		{
			if(entities.ContainsKey(entity))
				return;
			foreach(var type in components.Keys)
			{
				if(!entity.HasComponent(type))
					return;
			}
			var family = typeof(TFamilyMember);
			var member = pooled.Remove();
			member.Entity = entity;
			foreach(var type in components.Keys)
			{
				family.GetProperty(components[type]).SetValue(member, entity.GetComponent(type));
			}
			members.Add(member);
			entities.Add(entity, member);
			Message<IFamilyMemberAddMessage>(new FamilyMemberAddMessage(member));
		}

		private void Remove(IEntity entity)
		{
			if(!entities.ContainsKey(entity))
				return;
			var member = entities[entity];
			entities.Remove(entity);
			members.Remove(member);
			Message<IFamilyMemberRemoveMessage>(new FamilyMemberRemoveMessage(member));

			if(Engine == null || Engine.UpdateState == UpdatePhase.None)
			{
				DisposeMember(member);
			}
			else
			{
				removed.Push(member);
				Engine.AddListener<IUpdatePhaseMessage>(PoolMembers);
			}
		}

		private void PoolMembers(IUpdatePhaseMessage message)
		{
			//Only listen when it comes form the source.
			if(!message.AtMessenger)
				return;
			//Clean up update listener.
			message.Messenger.RemoveListener<IUpdatePhaseMessage>(PoolMembers);
			while(removed.Count > 0)
			{
				DisposeMember(removed.Pop());
			}

		}

		private void DisposeMember(TFamilyMember member)
		{
			var family = typeof(TFamilyMember);
			member.Entity = null;
			foreach(var type in components.Keys)
			{
				family.GetProperty(components[type]).SetValue(member, null);
			}
			pooled.Add(member);
		}
	}
}
