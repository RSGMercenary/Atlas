using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Interfaces
{
	interface IDispose<T>:IDisposable
	{
		bool IsDisposing { get; }
		ISignal<T> Disposed { get; }
	}
}
