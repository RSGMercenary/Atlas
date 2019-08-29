using Atlas.Core.Signals;

namespace Atlas.Core.Messages
{
	public class HierarchySlot<TMessage, T> : Slot<TMessage>
		where TMessage : IMessage<T>
		where T : class, IHierarchyMessenger<T>
	{
		public MessageFlow Messenger { get; set; } = MessageFlow.Self;

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
			if(Messenger == MessageFlow.All)
				return true;
			if(Messenger.HasFlag(MessageFlow.Self) && current == first)
				return true;
			if(Messenger.HasFlag(MessageFlow.Parent) && current.Parent == first)
				return true;
			if(Messenger.HasFlag(MessageFlow.Child) && current == first.Parent)
				return true;
			if(Messenger.HasFlag(MessageFlow.Sibling) && current.HasSibling(first))
				return true;
			if(Messenger.HasFlag(MessageFlow.Ancestor) && current.HasAncestor(first))
				return true;
			if(Messenger.HasFlag(MessageFlow.Descendent) && current.HasDescendant(first))
				return true;
			return false;
		}
	}
}