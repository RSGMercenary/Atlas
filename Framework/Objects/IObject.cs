using Atlas.Framework.Messages;
using System;

namespace Atlas.Framework.Objects
{
	public interface IObject : IMessageDispatcher, IDisposable
	{
		ObjectState State { get; }
	}
}
