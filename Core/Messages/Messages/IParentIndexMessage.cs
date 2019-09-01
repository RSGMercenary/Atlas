namespace Atlas.Core.Messages
{
	public interface IParentIndexMessage<out T> : IPropertyMessage<T, int>
				where T : IMessenger, IHierarchy<T>

	{
	}
}