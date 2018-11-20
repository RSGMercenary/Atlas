using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface IAutoDispose : IMessenger
	{
		bool AutoDispose { get; set; }
	}
}