using Atlas.Engine.Interfaces;
using System;
using System.Diagnostics;

namespace Atlas.Engine.Signals
{
	class Signal:SignalBase<ISlot, Action>, ISignal, IDispatch
	{
		public Signal()
		{

		}

		public bool Dispatch()
		{
			if(DispatchStart())
			{
				foreach(Slot slot in SlotsCopy)
				{
					try
					{
						slot.Listener.Invoke();
					}
					catch(Exception e)
					{
						Debug.WriteLine(e);
						//We remove the Slot so the Error doesn't inevitably happen again.
						Remove(slot.Listener);
					}
				}
				DispatchStop();
				return true;
			}
			return false;
		}

		override protected SlotBase CreateSlot()
		{
			return new Slot();
		}
	}
}