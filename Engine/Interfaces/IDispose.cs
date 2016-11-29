using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Interfaces
{
	interface IDispose<T>:IDisposable
	{
		bool IsDisposed { get; }
		ISignal<T, bool, bool> IsDisposedChanged { get; }
	}
}
