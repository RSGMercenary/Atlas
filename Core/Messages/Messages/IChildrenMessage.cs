namespace Atlas.Core.Messages
{
	public interface IChildrenMessage<T> : IMessage<T>
				where T : IMessenger, IHierarchy<T>

	{

	}
}