using Atlas.Core.Messages;
using Atlas.Core.Signals;

namespace Atlas.Core.Collections.Hierarchy
{
	public class HierarchySlot<TMessage, T> : Slot<TMessage>
		where TMessage : IMessage<T>
		where T : class, IMessenger, IHierarchy<T>
	{
		public Hierarchy Messenger { get; set; } = Hierarchy.Self;

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
			if(Messenger == Hierarchy.All)
				return true;
			if(Messenger.HasFlag(Hierarchy.Self) && current == first)
				return true;
			if(Messenger.HasFlag(Hierarchy.Parent) && current.Parent == first)
				return true;
			if(Messenger.HasFlag(Hierarchy.Child) && current == first.Parent)
				return true;
			if(Messenger.HasFlag(Hierarchy.Sibling) && current.HasSibling(first))
				return true;
			if(Messenger.HasFlag(Hierarchy.Ancestor) && current.HasAncestor(first))
				return true;
			if(Messenger.HasFlag(Hierarchy.Descendent) && current.HasDescendant(first))
				return true;
			return false;
		}
	}
}
