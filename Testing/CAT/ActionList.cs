using System;
using System.Collections.Generic;
using Atlas.Engine.Signals;

namespace Atlas.Testing.CAT
{
	class ActionList : IActionList
	{
		protected List<IAction> actions = new List<IAction>();
		private RunMode runMode = RunMode.Serial;
		private Signal<IActionList, RunMode, RunMode> runModeChanged = new Signal<IActionList, RunMode, RunMode>();

		private Signal<IReadOnlyAction, bool> isRunningChanged = new Signal<IReadOnlyAction, bool>();


		public ActionList()
		{

		}

		public void Add(IAction action)
		{

		}

		public bool IsRunning
		{
			get
			{
				return actions.Exists(action => { return action.IsRunning; });
			}
		}
		
		public RunMode RunMode
		{
			get { return runMode; }
			set
			{
				if(runMode == value)
					return;
				var previous = runMode;
				runMode = value;
				runModeChanged.Dispatch(this, value, previous);
			}
		}

		bool IAction.IsRunning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public ISignal<IReadOnlyAction, bool> IsRunningChanged => throw new NotImplementedException();
	}
}
