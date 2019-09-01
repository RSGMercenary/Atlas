namespace Atlas.Core.Messages
{
	public interface IChildrenMessage<out T> : IMessage<T>
				where T : IMessenger, IHierarchy<T>

	{

	}
}