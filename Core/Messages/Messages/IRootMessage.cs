namespace Atlas.Core.Messages
{
	public interface IRootMessage<out T> : IPropertyMessage<T, T>
				where T : IMessenger, IHierarchy<T>

	{
	}
}