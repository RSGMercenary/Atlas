namespace Atlas.Core.Messages
{
	public interface IParentMessage<out T> : IPropertyMessage<T, T>
				where T : IMessenger, IHierarchy<T>

	{
	}
}