namespace Atlas.Core.Messages
{
	public interface IMessage<out TMessenger>
		where TMessenger : IMessenger
	{
		TMessenger Messenger { get; }

		IMessenger CurrentMessenger { get; set; }
	}
}