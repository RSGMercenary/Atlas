using Atlas.ECS.Families;
using Atlas.ECS.Messages;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasFamilySystem<TFamilyMember> : AtlasFamilySystem<IFamilySystem, TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{

	}

	public abstract class AtlasFamilySystem<T, TFamilyMember> : AtlasSystem<T>, IFamilySystem<T, TFamilyMember>
		where T : class, IFamilySystem
		where TFamilyMember : IFamilyMember, new()
	{
		IReadOnlyFamily IReadOnlyFamilySystem.Family => Family;
		public IReadOnlyFamily<TFamilyMember> Family { get; private set; }
		public bool UpdateSleepingEntities { get; set; } = false;

		protected override void Updating(double deltaTime)
		{
			var updateSleepingEntities = UpdateSleepingEntities;
			foreach(var member in Family?.Members)
			{
				if(updateSleepingEntities || !member.Entity.IsSleeping)
					MemberUpdate(deltaTime, member);
			}
		}

		protected virtual void MemberUpdate(double deltaTime, TFamilyMember member) { }

		protected virtual void MemberAdded(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

		protected virtual void MemberRemoved(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

		protected override void AddFamilies()
		{
			base.AddFamilies();
			Family = Engine.AddFamily<TFamilyMember>();
			Family.AddListener<IFamilyMemberAddMessage<TFamilyMember>>(EntityAdded);
			Family.AddListener<IFamilyMemberRemoveMessage<TFamilyMember>>(EntityRemoved);
			foreach(var member in Family.Members)
				MemberAdded(Family, member);
		}

		protected override void RemoveFamilies()
		{
			Family.RemoveListener<IFamilyMemberAddMessage<TFamilyMember>>(EntityAdded);
			Family.RemoveListener<IFamilyMemberRemoveMessage<TFamilyMember>>(EntityRemoved);
			foreach(var member in Family.Members)
				MemberRemoved(Family, member);
			Engine.RemoveFamily<TFamilyMember>();
			Family = null; //TO-DO Pretty sure this is safe.
			base.RemoveFamilies();
		}

		private void EntityAdded(IFamilyMemberAddMessage<TFamilyMember> message)
		{
			MemberAdded(message.Messenger, message.Value);
		}

		private void EntityRemoved(IFamilyMemberRemoveMessage<TFamilyMember> message)
		{
			MemberRemoved(message.Messenger, message.Value);
		}
	}
}