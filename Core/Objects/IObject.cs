using Atlas.Core.Messages;
using System;

namespace Atlas.Core.Objects
{
	public interface IObject : IMessenger, IDisposable
	{
		ObjectState State { get; }
	}

	public interface IObject<T> : IObject, IMessenger<T>
		where T : IObject
	{
	}
}
