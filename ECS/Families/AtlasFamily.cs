using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.ECS.Messages;
using Atlas.ECS.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.ECS.Families
{
	sealed class AtlasFamily<TFamilyMember> : EngineObject<IReadOnlyFamily>, IFamily<TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{
		private Group<TFamilyMember> members = new Group<TFamilyMember>();
		private Dictionary<IEntity, TFamilyMember> entities = new Dictionary<IEntity, TFamilyMember>();
		private Dictionary<Type, string> components = new Dictionary<Type, string>();
		private Stack<TFamilyMember> removed = new Stack<TFamilyMember>();
		private Pool<TFamilyMember> pooled = new Pool<TFamilyMember>(() => new TFamilyMember());

		public AtlasFamily()
		{
			foreach(var property in typeof(TFamilyMember).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if(property.Name == "Entity")
					continue;
				components.Add(property.PropertyType, property.Name);
			}
		}

		public IReadOnlyGroup<TFamilyMember> Members { get { return members; } }

		IReadOnlyGroup<IFamilyMember> IReadOnlyFamily.Members
		{
			get { return members as IReadOnlyGroup<IFamilyMember>; }
		}

		public sealed override void Dispose()
		{
			if(State != ObjectState.Composed)
				return;
			//Can't destroy Family mid-update.
			if(Engine != null && Engine.UpdateState != TimeStep.None)
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

		public sealed override IEngine Engine
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
			Dispatch<IFamilyMemberAddMessage<TFamilyMember>>(new FamilyMemberAddMessage<TFamilyMember>(this, member));
		}

		private void Remove(IEntity entity)
		{
			if(!entities.ContainsKey(entity))
				return;
			var member = entities[entity];
			entities.Remove(entity);
			members.Remove(member);
			Dispatch<IFamilyMemberRemoveMessage<TFamilyMember>>(new FamilyMemberRemoveMessage<TFamilyMember>(this, member));

			if(Engine == null || Engine.UpdateState == TimeStep.None)
			{
				DisposeMember(member);
			}
			else
			{
				removed.Push(member);
				Engine.AddListener<IUpdateStateMessage<IEngine>>(PoolMembers);
			}
		}

		private void PoolMembers(IUpdateStateMessage<IEngine> message)
		{
			//Clean up update listener.
			message.Messenger.RemoveListener<IUpdateStateMessage<IEngine>>(PoolMembers);
			while(removed.Count > 0)
				DisposeMember(removed.Pop());
		}

		private void DisposeMember(TFamilyMember member)
		{
			var family = typeof(TFamilyMember);
			member.Entity = null;
			foreach(var type in components.Keys)
				family.GetProperty(components[type]).SetValue(member, null);
			pooled.Add(member);
		}
	}
}
