namespace Atlas.Core.Objects
{
	public interface IAutoDestroyObject : IObject
	{
		bool AutoDestroy { get; set; }
	}

	public interface IAutoDestroyObject<T> : IAutoDestroyObject, IObject<T>
		where T : IAutoDestroyObject
	{
	}
}