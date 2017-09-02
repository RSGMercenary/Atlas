using Atlas.Engine.Signals;

namespace Atlas.Testing.CAT
{
	interface IReadOnlyAction
	{
		bool IsRunning { get; }
		ISignal<IReadOnlyAction, bool> IsRunningChanged { get; }
	}

	interface IAction : IReadOnlyAction
	{
		new bool IsRunning { get; set; }


	}
}
