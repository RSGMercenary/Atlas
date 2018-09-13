namespace Atlas.Framework.Messages
{
	public class Message
	{
		public IMessageDispatcher Messenger { get; }
		public IMessageDispatcher CurrentMessenger { get; set; }

		public Message(IMessageDispatcher messenger)
		{
			Messenger = messenger;
		}
	}

	public class Message<TMessenger> : Message, IMessage<TMessenger>
		where TMessenger : IMessageDispatcher
	{
		public Message(TMessenger messenger) : base(messenger)
		{

		}

		public new TMessenger Messenger
		{
			get { return (TMessenger)base.Messenger; }
		}
	}
}