using Atlas.ECS.Components;
using Atlas.ECS.Families;
using Atlas.ECS.Families.Messages;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasFamilySystem<TFamilyMember> : AtlasSystem, IFamilySystem<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		IFamily IFamilySystem.Family => Family;
		public IFamily<TFamilyMember> Family { get; private set; }
		public bool UpdateSleepingEntities { get; protected set; } = false;

		protected sealed override void SystemUpdate(float deltaTime)
		{
			FamilyUpdate(deltaTime);
		}

		protected virtual void FamilyUpdate(float deltaTime)
		{
			var updateSleepingEntities = UpdateSleepingEntities;
			foreach(var member in Family)
			{
				if(updateSleepingEntities || !member.Entity.IsSleeping)
					MemberUpdate(deltaTime, member);
			}
		}

		protected virtual void MemberUpdate(float deltaTime, TFamilyMember member) { }

		protected virtual void MemberAdded(IFamily<TFamilyMember> family, TFamilyMember member) { }

		protected virtual void MemberRemoved(IFamily<TFamilyMember> family, TFamilyMember member) { }

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			Family = engine.AddFamily<TFamilyMember>();
			Family.AddListener<IFamilyMemberAddMessage<TFamilyMember>>(MemberAdded);
			Family.AddListener<IFamilyMemberRemoveMessage<TFamilyMember>>(MemberRemoved);
			foreach(var member in Family.Members)
				MemberAdded(Family, member);
		}

		protected override void RemovingEngine(IEngine engine)
		{
			Family.RemoveListener<IFamilyMemberAddMessage<TFamilyMember>>(MemberAdded);
			Family.RemoveListener<IFamilyMemberRemoveMessage<TFamilyMember>>(MemberRemoved);
			foreach(var member in Family.Members)
				MemberRemoved(Family, member);
			engine.RemoveFamily<TFamilyMember>();
			Family = null;
			base.RemovingEngine(engine);
		}

		private void MemberAdded(IFamilyMemberAddMessage<TFamilyMember> message)
		{
			MemberAdded(message.Messenger, message.Value);
		}

		private void MemberRemoved(IFamilyMemberRemoveMessage<TFamilyMember> message)
		{
			MemberRemoved(message.Messenger, message.Value);
		}
	}
}