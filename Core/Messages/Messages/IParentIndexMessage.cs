namespace Atlas.Core.Messages
{
	public interface IParentIndexMessage<T> : IPropertyMessage<T, int>
				where T : IMessenger, IHierarchy<T>

	{
	}
}