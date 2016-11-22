namespace Atlas.Interfaces
{
	interface IDispose
	{
		void Dispose();
		bool IsDisposed { get; }
		//ISignal<T, bool, bool> IsDisposedChanged { get; }
	}
}
