using Atlas.Framework.Messages;
using System;

namespace Atlas.Framework.Objects
{
	public interface IObject : IMessageDispatcher, IDisposable
	{
		ObjectState State { get; }
	}

	public interface IObject<T> : IObject, IMessageDispatcher<T>
		where T : IObject<T>
	{

	}
}
