using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface ISleep : IMessenger
	{
		bool IsSleeping { get; set; }
		int Sleeping { get; }
	}
}
