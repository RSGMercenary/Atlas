namespace Atlas.Core.Objects
{
	public interface IUpdateObject : IObject
	{
		bool IsUpdating { get; }
	}

	public interface IUpdateObject<T> : IUpdateObject, IObject<T>
		where T : IUpdateObject
	{
	}
}
