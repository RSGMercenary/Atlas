using Atlas.Core.Messages;
using Atlas.ECS.Components;

namespace Atlas.ECS.Objects
{
	public interface IObject : IMessenger
	{
		IEngine Engine { get; set; }
	}

	public interface IObject<in T> : IMessenger<T>, IObject
		where T : IObject
	{

	}
}
