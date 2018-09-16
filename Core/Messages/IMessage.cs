namespace Atlas.Core.Messages
{
	public interface IMessage
	{
		IMessenger Messenger { get; }
		IMessenger CurrentMessenger { get; set; }
	}

	public interface IMessage<TMessenger> : IMessage
		where TMessenger : IMessenger
	{
		/// <summary>
		/// The TMessenger that first created and sent the Message.
		/// </summary>
		new TMessenger Messenger { get; }
	}
}
