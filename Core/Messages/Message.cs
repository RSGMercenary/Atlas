namespace Atlas.Core.Messages
{
	public class Message<TMessenger> : IMessage<TMessenger>
		where TMessenger : class, IMessenger
	{
		private TMessenger messenger;

		public TMessenger Messenger
		{
			get => messenger;
			private set => messenger = messenger ?? value;
		}

		public TMessenger CurrentMessenger { get; private set; }

		#region IMessage
		IMessenger IMessage.Messenger
		{
			get => Messenger;
			set => Messenger = (TMessenger)value;
		}

		IMessenger IMessage.CurrentMessenger
		{
			get => CurrentMessenger;
			set => CurrentMessenger = (TMessenger)value;
		}
		#endregion
	}
}