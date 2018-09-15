using Atlas.ECS.Components;
using Atlas.ECS.Families;
using Atlas.ECS.Messages;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasFamilySystem<TFamilyMember> : AtlasSystem, IFamilySystem<TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{
		IReadOnlyFamily IFamilySystem.Family => Family;
		public IReadOnlyFamily<TFamilyMember> Family { get; private set; }
		public bool UpdateSleepingEntities { get; protected set; } = false;

		public AtlasFamilySystem()
		{

		}



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

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			Family = engine.AddFamily<TFamilyMember>();
			Family.AddListener<IFamilyMemberAddMessage>(EntityAdded);
			Family.AddListener<IFamilyMemberRemoveMessage>(EntityRemoved);
			foreach(var member in Family.Members)
				MemberAdded(Family, member);
		}

		protected override void RemovingEngine(IEngine engine)
		{
			Family.RemoveListener<IFamilyMemberAddMessage>(EntityAdded);
			Family.RemoveListener<IFamilyMemberRemoveMessage>(EntityRemoved);
			foreach(var member in Family.Members)
				MemberRemoved(Family, member);
			engine.RemoveFamily<TFamilyMember>();
			Family = null; //TO-DO Pretty sure this is safe.
			base.RemovingEngine(engine);
		}

		private void EntityAdded(IFamilyMemberAddMessage message)
		{
			MemberAdded(message.Messenger as IReadOnlyFamily<TFamilyMember>, (TFamilyMember)message.Value);
		}

		private void EntityRemoved(IFamilyMemberRemoveMessage message)
		{
			MemberRemoved(message.Messenger as IReadOnlyFamily<TFamilyMember>, (TFamilyMember)message.Value);
		}
	}
}