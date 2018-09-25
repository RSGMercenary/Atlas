namespace Atlas.Core.Messages
{
	public class Message : IMessage
	{
		public IMessenger Messenger { get; }
		public IMessenger CurrentMessenger { get; set; }

		public Message(IMessenger messenger)
		{
			Messenger = messenger;
		}
	}

	public class Message<TMessenger> : Message, IMessage<TMessenger>
		where TMessenger : IMessenger
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