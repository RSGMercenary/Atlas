namespace Atlas.Core.Messages
{
	public class Message<TMessenger> : IMessage<TMessenger>
		where TMessenger : IMessenger
	{
		private TMessenger messenger;

		public TMessenger Messenger
		{
			get => messenger;
			private set => messenger = messenger != null ? messenger : value;
		}

		public TMessenger CurrentMessenger { get; private set; }

		#region IMessage
		IMessenger IMessage.Messenger
		{
			set => Messenger = (TMessenger)value;
		}

		IMessenger IMessage.CurrentMessenger
		{
			set => CurrentMessenger = (TMessenger)value;
		}
		#endregion
	}
}