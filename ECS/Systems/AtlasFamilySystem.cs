using Atlas.ECS.Components;
using Atlas.ECS.Families;
using Atlas.ECS.Families.Messages;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasFamilySystem<TFamilyMember> : AtlasSystem, IFamilySystem<TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{
		IReadOnlyFamily IFamilySystem.Family => Family;
		public IReadOnlyFamily<TFamilyMember> Family { get; private set; }
		public bool UpdateSleepingEntities { get; protected set; } = false;

		protected sealed override void Updating(double deltaTime)
		{
			FamilyUpdate(deltaTime);
		}

		protected virtual void FamilyUpdate(double deltaTime)
		{
			var updateSleepingEntities = UpdateSleepingEntities;
			foreach(var member in Family.Members)
			{
				if(updateSleepingEntities || !member.Entity.IsSleeping)
					MemberUpdate(deltaTime, member);
			}
		}

		protected virtual void MemberUpdate(double deltaTime, TFamilyMember member) { }

		protected virtual void MemberAdded(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

		protected virtual void MemberRemoved(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

		protected override void AddFamilies(IEngine engine)
		{
			base.AddFamilies(engine);
			Family = engine.AddFamily<TFamilyMember>();
			Family.AddListener<IFamilyMemberAddMessage<TFamilyMember>>(MemberAdded);
			Family.AddListener<IFamilyMemberRemoveMessage<TFamilyMember>>(MemberRemoved);
			foreach(var member in Family.Members)
				MemberAdded(Family, member);
		}

		protected override void RemoveFamilies(IEngine engine)
		{
			Family.RemoveListener<IFamilyMemberAddMessage<TFamilyMember>>(MemberAdded);
			Family.RemoveListener<IFamilyMemberRemoveMessage<TFamilyMember>>(MemberRemoved);
			foreach(var member in Family.Members)
				MemberRemoved(Family, member);
			engine.RemoveFamily<TFamilyMember>();
			Family = null;
			base.RemoveFamilies(engine);
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