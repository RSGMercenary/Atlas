using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface IAutoDestroy : IMessenger
	{
		bool AutoDestroy { get; set; }
	}

	public interface IAutoDestroy<T> : IAutoDestroy, IMessenger<T>
		where T : IAutoDestroy
	{
	}
}