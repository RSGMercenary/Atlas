namespace Atlas.Core.Messages
{
	public interface IMessage
	{
		IMessenger Messenger { set; }

		IMessenger CurrentMessenger { set; }
	}

	public interface IMessage<TMessenger> : IMessage
		where TMessenger : IMessenger
	{
		new TMessenger Messenger { get; }

		new TMessenger CurrentMessenger { get; }
	}
}