namespace Atlas.Engine.Messages
{
	public interface IMessage : IMessageBase
	{
		new object Messenger { get; }
		new object CurrentMessenger { get; }
	}

	public interface IMessage<TMessenger> : IMessage, IMessageBase<TMessenger>
	{
		/// <summary>
		/// The TMessenger that first created and sent the Message.
		/// </summary>
		new TMessenger Messenger { get; }

		/// <summary>
		/// The IMessageDispatcher that is currently processing the Message.
		/// </summary>
		new object CurrentMessenger { get; }
	}
}
