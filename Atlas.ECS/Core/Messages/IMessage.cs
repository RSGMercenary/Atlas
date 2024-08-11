namespace Atlas.Core.Messages;

public interface IMessage
{
	IMessenger Messenger { get; set; }

	IMessenger CurrentMessenger { get; set; }
}

public interface IMessage<out TMessenger> : IMessage
	where TMessenger : IMessenger
{
	new TMessenger Messenger { get; }

	new TMessenger CurrentMessenger { get; }
}