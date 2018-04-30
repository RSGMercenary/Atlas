using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Families;
using Atlas.Engine.Messages;

namespace Atlas.Engine.Systems
{
	public abstract class AtlasFamilySystem<TFamilyMember> : AtlasSystem, IFamilySystem
		where TFamilyMember : IFamilyMember, new()
	{
		private IFamily<TFamilyMember> family;
		private UpdatePhase updateMode = UpdatePhase.Update;
		private bool updateSleepingEntities = false;

		public AtlasFamilySystem()
		{

		}

		public UpdatePhase UpdateMode
		{
			get { return updateMode; }
			set
			{
				if(value == updateMode)
					return;
				updateMode = value;
			}
		}

		public bool UpdateSleepingEntities
		{
			get { return updateSleepingEntities; }
			set
			{
				if(value == updateSleepingEntities)
					return;
				updateSleepingEntities = value;
			}
		}

		sealed override protected void Updating(double deltaTime)
		{
			if(updateMode == UpdatePhase.Update)
				FamilyUpdate(deltaTime);
		}

		sealed protected override void FixedUpdating(double deltaTime)
		{
			if(updateMode == UpdatePhase.FixedUpdate)
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

		virtual protected void MemberUpdate(double deltaTime, TFamilyMember member)
		{

		}

		virtual protected void MemberAdded(IFamily family, TFamilyMember member)
		{

		}

		virtual protected void MemberRemoved(IFamily family, TFamilyMember member)
		{

		}

		override protected void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			family = engine.AddFamily<TFamilyMember>();
			family.AddListener<IFamilyEntityAddMessage>(EntityAdded);
			family.AddListener<IFamilyMemberRemoveMessage>(EntityRemoved);
			foreach(var member in family.Members)
			{
				MemberAdded(family, member);
			}
		}

		override protected void RemovingEngine(IEngine engine)
		{
			family.RemoveListener<IFamilyEntityAddMessage>(EntityAdded);
			family.RemoveListener<IFamilyMemberRemoveMessage>(EntityRemoved);
			foreach(var member in family.Members)
			{
				MemberRemoved(family, member);
			}
			engine.RemoveFamily<TFamilyMember>();
			family = null; //TO-DO Pretty sure this is safe.
			base.RemovingEngine(engine);
		}

		private void EntityAdded(IFamilyEntityAddMessage message)
		{
			MemberAdded(message.Messenger, (TFamilyMember)message.Value);
		}

		private void EntityRemoved(IFamilyMemberRemoveMessage message)
		{
			MemberRemoved(message.Messenger, (TFamilyMember)message.Value);
		}
	}
}