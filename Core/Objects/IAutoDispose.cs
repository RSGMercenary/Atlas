using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface IAutoDispose : IMessenger
	{
		bool AutoDispose { get; set; }
	}

	public interface IAutoDispose<T> : IAutoDispose, IMessenger<T>
		where T : IAutoDispose
	{
	}
}