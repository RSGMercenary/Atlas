namespace Atlas.Core.Messages
{
	public interface IChildRemoveMessage<out T> : IKeyValueMessage<T, int, T>
				where T : IMessenger, IHierarchy<T>

	{

	}
}