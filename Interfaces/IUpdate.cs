using Atlas.Signals;

namespace Atlas.Interfaces
{
	interface IUpdate<T>
	{
		void Update();

		bool IsUpdating { get; }
		Signal<T, bool, bool> IsUpdatingChanged { get; }
	}
}
