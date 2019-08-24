namespace Atlas.Core.Objects
{
	public interface IReadOnlyPriority
	{
		int Priority { get; }
	}

	public interface IPriority : IReadOnlyPriority
	{
		new int Priority { get; set; }
	}
}
