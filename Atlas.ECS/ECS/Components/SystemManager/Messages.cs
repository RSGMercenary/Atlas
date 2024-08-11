using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Components.SystemManager;

#region Interfaces
public interface ISystemTypeAddMessage : IValueMessage<ISystemManager, Type> { }

public interface ISystemTypeRemoveMessage : IValueMessage<ISystemManager, Type> { }
#endregion

#region Classes
class SystemTypeAddMessage : ValueMessage<ISystemManager, Type>, ISystemTypeAddMessage
{
	public SystemTypeAddMessage(Type value) : base(value) { }
}

class SystemTypeRemoveMessage : ValueMessage<ISystemManager, Type>, ISystemTypeRemoveMessage
{
	public SystemTypeRemoveMessage(Type value) : base(value) { }
}
#endregion