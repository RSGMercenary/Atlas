namespace Atlas.Engine.Messages
{
	public interface IMessageBase
	{
		object Messenger { get; set; }
		object CurrentMessenger { get; set; }
		bool AtMessenger { get; }
	}

	public interface IMessageBase<TMessenger> : IMessageBase
	{
		new TMessenger Messenger { get; set; }
	}
}
