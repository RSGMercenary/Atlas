using System;

namespace Atlas.Interfaces
{
	interface IDispose:IDisposable
	{
		bool IsDisposed { get; }
		//ISignal<T, bool, bool> IsDisposedChanged { get; }
	}
}
