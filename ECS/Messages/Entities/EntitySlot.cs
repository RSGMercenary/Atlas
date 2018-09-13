using Atlas.Core.Signals;
using Atlas.ECS.Entities;

namespace Atlas.Core.Messages.Signals
{
	public class EntitySlot<TMessage> : Slot<TMessage>
		where TMessage : IMessage
	{
		public EntityHierarchy Hierarchy { get; set; } = EntityHierarchy.Self;

		public override bool Dispatch(TMessage message)
		{
			var first = message.Messenger as IEntity;
			var current = message.CurrentMessenger as IEntity;
			if(first == null || current == null)
				return base.Dispatch(message);

			if(Hierarchy.HasFlag(EntityHierarchy.All))
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(EntityHierarchy.Self) && current == first)
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(EntityHierarchy.Parent) && current.Parent == first)
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(EntityHierarchy.Child) && current == first.Parent)
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(EntityHierarchy.Sibling) && current.HasSibling(first))
				return Dispatch(message);
			if(Hierarchy.HasFlag(EntityHierarchy.Ancestor) && current.HasAncestor(first))
				return Dispatch(message);
			if(Hierarchy.HasFlag(EntityHierarchy.Descendent) && current.HasDescendant(first))
				return Dispatch(message);
			return false;
		}
	}
}