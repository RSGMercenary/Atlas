namespace Atlas.Core.Messages
{
	public interface IMessage
	{
		IMessageDispatcher Messenger { get; }
		IMessageDispatcher CurrentMessenger { get; set; }
	}

	public interface IMessage<TMessenger> : IMessage
		where TMessenger : IMessageDispatcher
	{
		/// <summary>
		/// The TMessenger that first created and sent the Message.
		/// </summary>
		new TMessenger Messenger { get; }
	}
}
