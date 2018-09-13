using System;

namespace Atlas.Core.Objects
{
	[Flags]
	public enum ObjectState
	{
		Initializing = 1,
		Initialized = 2,
		Disposing = 4,
		Disposed = 8
	}
}
