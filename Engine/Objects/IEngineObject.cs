using Atlas.Engine.Components;
using Atlas.Engine.Messages;

namespace Atlas.Engine
{
    public interface IEngineObject : IMessageDispatcher
    {
        IEngine Engine { get; set; }
        EngineObjectState State { get; }

        bool Destroy();
    }
}
