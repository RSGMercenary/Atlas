using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.Core.Objects.Update;
using Atlas.Core.Utilites;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.ECS.Families
{
	sealed class AtlasFamily<TFamilyMember> : Messenger<IReadOnlyFamily<TFamilyMember>>, IFamily<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		#region Fields

		private IEngine engine;

		//Reflection Fields
		private readonly FieldInfo entityField;
		private readonly Dictionary<Type, FieldInfo> components = new Dictionary<Type, FieldInfo>();

		//Family Members
		private readonly Group<TFamilyMember> members = new Group<TFamilyMember>();
		private readonly Dictionary<IEntity, TFamilyMember> entities = new Dictionary<IEntity, TFamilyMember>();

		//Pooling
		private readonly Stack<TFamilyMember> removed = new Stack<TFamilyMember>();
		private readonly Pool<TFamilyMember> pool = new InstancePool<TFamilyMember>();

		#endregion

		#region Compose / Dispose

		public AtlasFamily()
		{
			//Gets the private backing fields of the Entity and Component properties.
			foreach(var field in typeof(TFamilyMember).FindFields(BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if(field.FieldType == typeof(IEntity))
				{
					if(entityField == null)
						entityField = field;
					else
						throw new InvalidOperationException($"{typeof(TFamilyMember).Name} can't have multiple {nameof(IEntity)} properties.");
				}
				else if(typeof(IComponent).IsAssignableFrom(field.FieldType))
					components.Add(field.FieldType, field);
				else
					throw new InvalidOperationException($"{typeof(TFamilyMember).Name}'s {field.FieldType.Name} is not an {nameof(IComponent)}.");
			}
		}

		public sealed override void Dispose()
		{
			//Can't dispose Family mid-update.
			if(Engine != null || removed.Count > 0)
				return;
			base.Dispose();
		}

		#endregion

		#region Iteration

		public IEnumerator<TFamilyMember> GetEnumerator() => members.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		#region Engine

		public IEngine Engine
		{
			get => engine;
			set
			{
				if(value != null && Engine == null && value.HasFamily(this))
				{
					var previous = engine;
					engine = value;
					Message<IEngineMessage<IReadOnlyFamily<TFamilyMember>>>(new EngineMessage<IReadOnlyFamily<TFamilyMember>>(value, previous));
				}
				else if(value == null && Engine != null && !Engine.HasFamily(this))
				{
					var previous = engine;
					engine = value;
					Message<IEngineMessage<IReadOnlyFamily<TFamilyMember>>>(new EngineMessage<IReadOnlyFamily<TFamilyMember>>(value, previous));
					Dispose();
				}
			}
		}

		#endregion

		#region Get

		IReadOnlyGroup<IFamilyMember> IReadOnlyFamily.Members => Members;

		public IReadOnlyGroup<TFamilyMember> Members => members;

		IFamilyMember IReadOnlyFamily.GetMember(IEntity entity) => GetMember(entity);

		public TFamilyMember GetMember(IEntity entity) => entities.ContainsKey(entity) ? entities[entity] : null;

		#endregion

		#region Add

		public void AddEntity(IEntity entity) => Add(entity);

		public void AddEntity(IEntity entity, Type componentType)
		{
			if(components.ContainsKey(componentType))
				Add(entity);
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
			var member = SetMemberValues(pool.Get(), entity, true);
			members.Add(member);
			entities.Add(entity, member);
			Message<IFamilyMemberAddMessage<TFamilyMember>>(new FamilyMemberAddMessage<TFamilyMember>(member));
		}

		#endregion

		#region Remove

		public void RemoveEntity(IEntity entity) => Remove(entity);

		public void RemoveEntity(IEntity entity, Type componentType)
		{
			if(components.ContainsKey(componentType))
				Remove(entity);
		}

		private void Remove(IEntity entity)
		{
			if(!entities.ContainsKey(entity))
				return;
			var member = entities[entity];
			entities.Remove(entity);
			members.Remove(member);
			Message<IFamilyMemberRemoveMessage<TFamilyMember>>(new FamilyMemberRemoveMessage<TFamilyMember>(member));

			if((Engine?.UpdateState ?? TimeStep.None) == TimeStep.None)
			{
				DisposeMember(member);
			}
			else
			{
				removed.Push(member);
				Engine.AddListener<IUpdateStateMessage<IEngine>>(PoolMembers);
			}
		}

		#endregion

		#region Pooling

		private void PoolMembers(IUpdateStateMessage<IEngine> message)
		{
			//Clean up update listener.
			if(message.CurrentValue != TimeStep.None)
				return;
			message.Messenger.RemoveListener<IUpdateStateMessage<IEngine>>(PoolMembers);
			while(removed.Count > 0)
				DisposeMember(removed.Pop());
			if(Engine == null)
				Dispose();
		}

		private void DisposeMember(TFamilyMember member)
		{
			pool.Release(SetMemberValues(member, null, false));
		}

		#endregion

		#region Helpers

		private TFamilyMember SetMemberValues(TFamilyMember member, IEntity entity, bool add)
		{
			entityField.SetValue(member, entity);
			foreach(var type in components.Keys)
				components[type].SetValue(member, add ? entity.GetComponent(type) : null);
			return member;
		}

		public void SortMembers(Sort sort, Func<TFamilyMember, TFamilyMember, int> compare)
		{
			Sorter.Get<TFamilyMember>(sort).Invoke(members, compare);
		}

		#endregion
	}
}