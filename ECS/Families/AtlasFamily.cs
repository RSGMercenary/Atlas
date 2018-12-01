using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.ECS.Families.Messages;
using Atlas.ECS.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.ECS.Families
{
	sealed class AtlasFamily<TFamilyMember> : AtlasObject, IFamily<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		private readonly Group<TFamilyMember> members = new Group<TFamilyMember>();
		private readonly Dictionary<IEntity, TFamilyMember> entities = new Dictionary<IEntity, TFamilyMember>();
		private readonly Dictionary<Type, string> components = new Dictionary<Type, string>();
		private readonly Stack<TFamilyMember> removed = new Stack<TFamilyMember>();
		private readonly Pool<TFamilyMember> pool = new Pool<TFamilyMember>();

		public AtlasFamily()
		{
			foreach(var property in typeof(TFamilyMember).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if(property.Name == "Entity")
					continue;
				components.Add(property.PropertyType, property.Name);
			}
		}

		public sealed override void Dispose()
		{
			//Can't destroy Family mid-update.
			if(removed.Count > 0)
				return;
			base.Dispose();
		}

		protected override void Disposing()
		{
			//TO-DO
			//Do some clean up maybe? Or let the GC handle it.
			base.Disposing();
		}

		public IReadOnlyGroup<TFamilyMember> Members { get { return members; } }

		IReadOnlyGroup<IFamilyMember> IReadOnlyFamily.Members
		{
			get { return members; }
		}

		public IEnumerator<TFamilyMember> GetEnumerator()
		{
			return members.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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

		protected override void RemovingEngine(IEngine engine)
		{
			base.RemovingEngine(engine);
			Dispose();
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
			var member = pool.Remove();
			member.Entity = entity;
			foreach(var type in components.Keys)
			{
				family.GetProperty(components[type]).SetValue(member, entity.GetComponent(type));
			}
			members.Add(member);
			entities.Add(entity, member);
			Message<IFamilyMemberAddMessage<TFamilyMember>>(new FamilyMemberAddMessage<TFamilyMember>(this, member));
		}

		private void Remove(IEntity entity)
		{
			if(!entities.ContainsKey(entity))
				return;
			var member = entities[entity];
			entities.Remove(entity);
			members.Remove(member);
			Message<IFamilyMemberRemoveMessage<TFamilyMember>>(new FamilyMemberRemoveMessage<TFamilyMember>(this, member));

			if(Engine == null || Engine.UpdateState == TimeStep.None)
			{
				DisposeMember(member);
			}
			else
			{
				removed.Push(member);
				Engine.AddListener<IUpdateStateMessage>(PoolMembers);
			}
		}

		private void PoolMembers(IUpdateStateMessage message)
		{
			//Clean up update listener.
			if(message.CurrentValue != TimeStep.None)
				return;
			message.Messenger.RemoveListener<IUpdateStateMessage>(PoolMembers);
			while(removed.Count > 0)
				DisposeMember(removed.Pop());
			if(Engine == null)
				Dispose();
		}

		private void DisposeMember(TFamilyMember member)
		{
			var family = typeof(TFamilyMember);
			member.Entity = null;
			foreach(var type in components.Keys)
				family.GetProperty(components[type]).SetValue(member, null);
			pool.Add(member);
		}
	}
}
