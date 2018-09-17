namespace Atlas.Core.Objects
{
	public interface IUpdateStateObject : IObject
	{
		TimeStep UpdateState { get; }
	}

	public interface IUpdateStateObject<T> : IUpdateStateObject, IObject<T>
		where T : IUpdateStateObject
	{
	}
}
