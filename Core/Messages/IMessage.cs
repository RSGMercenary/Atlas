namespace Atlas.Core.Messages
{
	public interface IMessage
	{
		/// <summary>
		/// The TMessenger that first created and sent the Message.
		/// </summary>
		IMessenger Messenger { get; }
		IMessenger CurrentMessenger { get; set; }
	}

	public interface IMessage<out TMessenger> : IMessage
		where TMessenger : IMessenger
	{
		new TMessenger Messenger { get; }
	}
}
