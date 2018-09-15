namespace Atlas.Core.Objects
{
	public interface IAutoDestroyObject : IObject
	{
		bool AutoDestroy { get; set; }
	}
}
