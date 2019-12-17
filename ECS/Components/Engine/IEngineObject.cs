using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Engine
{
	public interface IEngineObject : IMessenger
	{
		IEngine Engine { get; set; }
	}

	public interface IEngineObject<T> : IMessenger<T>, IEngineObject where T : IEngineObject { }
}