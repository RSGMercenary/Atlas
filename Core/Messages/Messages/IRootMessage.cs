namespace Atlas.Core.Messages
{
	public interface IRootMessage<T> : IPropertyMessage<T, T>
				where T : IMessenger, IHierarchy<T>

	{
	}
}