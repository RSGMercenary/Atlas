namespace Atlas.Core.Messages
{
	public class Message<TMessenger> : IMessage<TMessenger>
		where TMessenger : IMessenger
	{
		IMessenger IMessage.Messenger => Messenger;
		public TMessenger Messenger { get; }
		public IMessenger CurrentMessenger { get; set; }

		public Message(TMessenger messenger)
		{
			Messenger = messenger;
		}
	}
}