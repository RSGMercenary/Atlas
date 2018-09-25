using Atlas.Core.Messages;
using Atlas.ECS.Components;

namespace Atlas.ECS.Objects
{
	public interface IObject : IMessenger
	{
		IEngine Engine { get; set; }
		ObjectState State { get; }
	}

	public interface IObject<T> : IObject, IMessenger<T>
		where T : IObject
	{
	}
}
