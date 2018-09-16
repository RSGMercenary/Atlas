using Atlas.Core.Messages;
using Atlas.Core.Signals;
using Atlas.ECS.Entities;

namespace Atlas.Core.Collections.Hierarchy
{
	public class HierarchySlot<TMessage> : Slot<TMessage>
		where TMessage : IMessage
	{
		public MessageHierarchy Hierarchy { get; set; } = MessageHierarchy.Self;

		public override bool Dispatch(TMessage message)
		{
			var first = message.Messenger as IEntity;
			var current = message.CurrentMessenger as IEntity;
			if(first == null || current == null)
				return base.Dispatch(message);

			if(Hierarchy.HasFlag(MessageHierarchy.All))
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(MessageHierarchy.Self) && current == first)
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(MessageHierarchy.Parent) && current.Parent == first)
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(MessageHierarchy.Child) && current == first.Parent)
				return base.Dispatch(message);
			if(Hierarchy.HasFlag(MessageHierarchy.Sibling) && current.HasSibling(first))
				return Dispatch(message);
			if(Hierarchy.HasFlag(MessageHierarchy.Ancestor) && current.HasAncestor(first))
				return Dispatch(message);
			if(Hierarchy.HasFlag(MessageHierarchy.Descendent) && current.HasDescendant(first))
				return Dispatch(message);
			return false;
		}
	}
}