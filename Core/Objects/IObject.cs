using Atlas.Core.Messages;
using System;

namespace Atlas.Core.Objects
{
	public interface IObject : IMessageDispatcher, IDisposable
	{
		ObjectState State { get; }
	}
}
