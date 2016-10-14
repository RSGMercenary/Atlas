namespace Atlas.Interfaces
{
	interface IDispose
	{
		void Dispose();

		bool IsDisposed { get; }
		bool IsDisposedWhenUnmanaged { get; set; }
	}
}
