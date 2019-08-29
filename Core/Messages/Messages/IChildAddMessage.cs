namespace Atlas.Core.Messages
{
	public interface IChildAddMessage<T> : IKeyValueMessage<T, int, T>
		where T : IMessenger, IHierarchy<T>
	{

	}
}