using Atlas.Core.Messages;
using Atlas.Core.Signals;

namespace Atlas.Core.Collections.Hierarchy
{
	public class HierarchySlot<TMessage, T> : Slot<TMessage>
		where TMessage : IMessage<T>
		where T : class, IMessenger, IHierarchy<T>
	{
		public Tree Messenger { get; set; } = Tree.Self;

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
			if(Messenger == Tree.All)
				return true;
			if(Messenger.HasFlag(Tree.Self) && current == first)
				return true;
			if(Messenger.HasFlag(Tree.Parent) && current.Parent == first)
				return true;
			if(Messenger.HasFlag(Tree.Child) && current == first.Parent)
				return true;
			if(Messenger.HasFlag(Tree.Sibling) && current.HasSibling(first))
				return true;
			if(Messenger.HasFlag(Tree.Ancestor) && current.HasAncestor(first))
				return true;
			if(Messenger.HasFlag(Tree.Descendent) && current.HasDescendant(first))
				return true;
			return false;
		}
	}
}