using Atlas.ECS.Components;
using Atlas.ECS.Families;
using Atlas.Framework.Messages;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasFamilySystem<TFamilyMember> : AtlasSystem, IFamilySystem
		where TFamilyMember : class, IFamilyMember, new()
	{
		private IReadOnlyFamily<TFamilyMember> family;
		public bool UpdateSleepingEntities { get; protected set; } = false;

		public AtlasFamilySystem()
		{

		}

		sealed override protected void Updating(double deltaTime)
		{
			FamilyUpdate(deltaTime);
		}

		virtual protected void FamilyUpdate(double deltaTime)
		{
			var updateSleepingEntities = UpdateSleepingEntities;
			foreach(var member in family.Members)
			{
				if(updateSleepingEntities || !member.Entity.IsSleeping)
					MemberUpdate(deltaTime, member);
			}
		}

		virtual protected void MemberUpdate(double deltaTime, TFamilyMember member) { }

		virtual protected void MemberAdded(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

		virtual protected void MemberRemoved(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

		override protected void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			family = engine.AddFamily<TFamilyMember>();
			family.AddListener<IFamilyMemberAddMessage>(EntityAdded);
			family.AddListener<IFamilyMemberRemoveMessage>(EntityRemoved);
			foreach(var member in family.Members)
			{
				MemberAdded(family, member);
			}
		}

		override protected void RemovingEngine(IEngine engine)
		{
			family.RemoveListener<IFamilyMemberAddMessage>(EntityAdded);
			family.RemoveListener<IFamilyMemberRemoveMessage>(EntityRemoved);
			foreach(var member in family.Members)
			{
				MemberRemoved(family, member);
			}
			engine.RemoveFamily<TFamilyMember>();
			family = null; //TO-DO Pretty sure this is safe.
			base.RemovingEngine(engine);
		}

		private void EntityAdded(IFamilyMemberAddMessage message)
		{
			MemberAdded(message.Messenger as IReadOnlyFamily<TFamilyMember>, message.Value as TFamilyMember);
		}

		private void EntityRemoved(IFamilyMemberRemoveMessage message)
		{
			MemberRemoved(message.Messenger as IReadOnlyFamily<TFamilyMember>, message.Value as TFamilyMember);
		}
	}
}