using Atlas.Core.Messages;
using Atlas.Signals;

namespace Atlas.Core.Collections.Hierarchy
{
	public class HierarchySlot<TMessage, T> : Slot<TMessage>
		where TMessage : IMessage<T>
		where T : class, IHierarchy<T>
	{
		public Relation Messenger { get; set; } = Relation.Self;

		public override bool Dispatch(TMessage item1)
		{
			if(CanDispatch(item1))
				return base.Dispatch(item1);
			return false;
		}

		private bool CanDispatch(TMessage message)
		{
			var first = message.Messenger;
			var current = message.CurrentMessenger;
			if(Messenger == Relation.All)
				return true;
			if(Messenger.HasFlag(Relation.Self) && current == first)
				return true;
			if(Messenger.HasFlag(Relation.Parent) && current.Parent == first)
				return true;
			if(Messenger.HasFlag(Relation.Child) && current == first.Parent)
				return true;
			if(Messenger.HasFlag(Relation.Sibling) && current.HasSibling(first))
				return true;
			if(Messenger.HasFlag(Relation.Ancestor) && current.HasAncestor(first))
				return true;
			if(Messenger.HasFlag(Relation.Descendent) && current.HasDescendant(first))
				return true;
			return false;
		}
	}
}