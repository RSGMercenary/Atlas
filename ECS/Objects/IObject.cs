using Atlas.Core.Messages;
using Atlas.ECS.Components.Engine;

namespace Atlas.ECS.Objects
{
	public interface IObject : IMessenger
	{
		IEngine Engine { get; set; }
	}

	public interface IObject<T> : IMessenger<T>, IObject where T : IObject { }
}