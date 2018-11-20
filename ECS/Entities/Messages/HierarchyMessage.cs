using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class HierarchyMessage : Message<IEntity>, IHierarchyMessage
	{
		private int value = 0;

		public HierarchyMessage(IEntity messenger) : base(messenger)
		{
		}

		public int Value
		{
			get { return value++; }
		}
	}
}
