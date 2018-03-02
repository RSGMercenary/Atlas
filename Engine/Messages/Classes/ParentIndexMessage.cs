using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class ParentIndexMessage : PropertyMessage<IEntity, int>, IParentIndexMessage
	{
		public ParentIndexMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
