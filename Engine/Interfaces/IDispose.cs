using System;

namespace Atlas.Engine.Interfaces
{
	interface IDispose:IDisposable
	{
		bool IsDisposed { get; }
		//ISignal<T, bool, bool> IsDisposedChanged { get; }
	}
}
