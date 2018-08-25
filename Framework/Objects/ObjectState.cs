using System;

namespace Atlas.Framework.Objects
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
