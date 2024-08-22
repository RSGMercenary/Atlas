using Atlas.Core.Messages;
using System;

namespace Atlas.Core.Collections.Pool;

#region Interfaces
public interface IPoolAddMessage : IKeyValueMessage<IPoolManager, Type, IPool> { }

public interface IPoolRemoveMessage : IKeyValueMessage<IPoolManager, Type, IPool> { }
#endregion

#region Classes
public class PoolAddMessage : KeyValueMessage<IPoolManager, Type, IPool>, IPoolAddMessage
{
	public PoolAddMessage(Type key, IPool value) : base(key, value) { }
}

public class PoolRemoveMessage : KeyValueMessage<IPoolManager, Type, IPool>, IPoolRemoveMessage
{
	public PoolRemoveMessage(Type key, IPool value) : base(key, value) { }
}
#endregion