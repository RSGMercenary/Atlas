namespace Atlas.Engine.Messages
{
	public class Message
	{
		private object messenger;
		private object currentMessenger;

		public object Messenger
		{
			get { return messenger; }
			set { messenger = messenger ?? value; }
		}

		public object CurrentMessenger
		{
			get { return currentMessenger; }
			set { currentMessenger = value; }
		}

		public bool AtMessenger
		{
			get { return (bool)messenger?.Equals(currentMessenger); }
		}
	}

	public class Message<TMessenger> : Message, IMessage<TMessenger>
	{
		public Message()
		{

		}

		new public TMessenger Messenger
		{
			get { return (TMessenger)base.Messenger; }
			set { base.Messenger = value; }
		}
	}
}