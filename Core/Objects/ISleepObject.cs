namespace Atlas.Core.Objects
{
	public interface ISleepObject : IObject
	{
		bool IsSleeping { get; set; }
		int Sleeping { get; }
	}
}
