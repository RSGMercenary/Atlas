using System;

namespace Atlas.Core.Objects
{
	[Flags]
	public enum ObjectState
	{
		Composing = 1,
		Composed = 2,
		Disposing = 4,
		Disposed = 8
	}
}
