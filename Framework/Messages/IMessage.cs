namespace Atlas.Framework.Messages
{
	public interface IMessageBase
	{
		IMessageDispatcher Messenger { get; set; }
		IMessageDispatcher CurrentMessenger { get; set; }
		bool AtMessenger { get; }
	}

	public interface IMessageBase<TMessenger> : IMessageBase
		where TMessenger : IMessageDispatcher
	{
		new TMessenger Messenger { get; set; }
	}

	public interface IMessage : IMessageBase
	{
		new IMessageDispatcher Messenger { get; }
		new IMessageDispatcher CurrentMessenger { get; }
	}

	public interface IMessage<TMessenger> : IMessage, IMessageBase<TMessenger>
		where TMessenger : IMessageDispatcher
	{
		/// <summary>
		/// The TMessenger that first created and sent the Message.
		/// </summary>
		new TMessenger Messenger { get; }

		/// <summary>
		/// The IMessageDispatcher that is currently processing the Message.
		/// </summary>
		new TMessenger CurrentMessenger { get; }
	}
}
