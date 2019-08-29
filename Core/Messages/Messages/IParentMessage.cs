namespace Atlas.Core.Messages
{
	public interface IParentMessage<T> : IPropertyMessage<T, T>
				where T : IMessenger, IHierarchy<T>

	{
	}
}