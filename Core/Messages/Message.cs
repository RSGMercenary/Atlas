namespace Atlas.Core.Messages
{
	public class Message<TMessenger> : IMessage<TMessenger>
		where TMessenger : IMessenger
	{
		public TMessenger Messenger { get; }

		public IMessenger CurrentMessenger { get; set; }

		public Message(TMessenger messenger)
		{
			Messenger = messenger;
		}
	}
}