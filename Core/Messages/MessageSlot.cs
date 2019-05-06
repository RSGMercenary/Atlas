using Atlas.Core.Signals;
using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	public class MessageSlot<TMessage> : Slot<TMessage>
		where TMessage : IMessage
	{
		public MessageFlow Flow { get; set; } = MessageFlow.Self;

		public override bool Dispatch(TMessage item1)
		{
			if(CanDispatch(item1))
				return base.Dispatch(item1);
			return false;
		}

		private bool CanDispatch(TMessage message)
		{
			var first = message.Messenger as IEntity;
			var current = message.CurrentMessenger as IEntity;
			if(first == null || current == null)
				return false;
			if(Flow == MessageFlow.All)
				return true;
			if(Flow.HasFlag(MessageFlow.Self) && current == first)
				return true;
			if(Flow.HasFlag(MessageFlow.Parent) && current.Parent == first)
				return true;
			if(Flow.HasFlag(MessageFlow.Child) && current == first.Parent)
				return true;
			if(Flow.HasFlag(MessageFlow.Sibling) && current.HasSibling(first))
				return true;
			if(Flow.HasFlag(MessageFlow.Ancestor) && current.HasAncestor(first))
				return true;
			if(Flow.HasFlag(MessageFlow.Descendent) && current.HasDescendant(first))
				return true;
			return false;
		}
	}
}
