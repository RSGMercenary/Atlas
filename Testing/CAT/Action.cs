using Atlas.Engine.Signals;

namespace Atlas.Testing.CAT
{
	class Action : IAction
	{
		private bool isRunning = false;
		private Signal<IReadOnlyAction, bool> isRunningChanged = new Signal<IReadOnlyAction, bool>();

		public bool IsRunning
		{
			get { return isRunning; }
			set
			{
				if(isRunning == value)
					return;
				isRunning = value;
				isRunningChanged.Dispatch(this, value);
			}
		}

		public ISignal<IReadOnlyAction, bool> IsRunningChanged
		{
			get { return isRunningChanged; }
		}

		virtual public void Start()
		{

		}

		virtual public void Stop()
		{

		}
	}
}
