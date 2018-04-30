using Atlas.Engine.Collections.EngineList;
using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.Engine.Families
{
	sealed class AtlasFamily<TFamilyMember> : EngineObject, IFamily<TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{
		private EngineList<TFamilyMember> members = new EngineList<TFamilyMember>();
		private Dictionary<IEntity, TFamilyMember> entities = new Dictionary<IEntity, TFamilyMember>();
		private Dictionary<Type, string> components = new Dictionary<Type, string>();
		private Stack<TFamilyMember> removed = new Stack<TFamilyMember>();
		private Stack<TFamilyMember> pooled = new Stack<TFamilyMember>();

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

		IReadOnlyEngineList<IFamilyMember> IFamily.Members
		{
			get { return members as IReadOnlyEngineList<IFamilyMember>; }
		}

		sealed public override bool Destroy()
		{
			if(State != EngineObjectState.Constructed)
				return false;
			//Can't destroy Family mid-update.
			if(Engine == null || Engine.UpdateState != UpdatePhase.None)
				return false;
			Engine = null;
			if(Engine == null)
				return base.Destroy();
			return false;
		}

		protected override void Destroying()
		{
			//TO-DO
			//Do some clean please! 
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

		private TFamilyMember GetMember()
		{
			return pooled.Count > 0 ? pooled.Pop() : new TFamilyMember();
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
			var member = GetMember();
			member.Entity = entity;
			foreach(var type in components.Keys)
			{
				family.GetProperty(components[type]).SetValue(member, entity.GetComponent(type));
			}
			members.Add(member);
			entities.Add(entity, member);
			Message<IFamilyEntityAddMessage>(new FamilyMemberAddMessage(member));
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
			pooled.Push(member);
		}
	}
}
