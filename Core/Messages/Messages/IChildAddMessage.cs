namespace Atlas.Core.Messages
{
	public interface IChildAddMessage<out T> : IKeyValueMessage<T, int, T>
		where T : IMessenger, IHierarchy<T>
	{

	}
}