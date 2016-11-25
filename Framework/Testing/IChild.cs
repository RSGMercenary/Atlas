namespace Atlas.Engine.Interfaces
{
	interface IChild<T>
	{
		T Parent { get; }
	}
}
