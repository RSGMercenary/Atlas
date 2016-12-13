using Atlas.Engine.Interfaces;
using System;
using System.Diagnostics;

namespace Atlas.Engine.Signals
{
	class Signal<T1>:SignalBase<ISlot<T1>, Action<T1>>, ISignal<T1>, IDispatch<T1>
	{
		public Signal()
		{

		}

		virtual public bool Dispatch(T1 item1)
		{
			if(DispatchStart())
			{
				foreach(Slot<T1> slot in SlotsCopy)
				{
					try
					{
						slot.Listener.Invoke(item1);
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
			return new Slot<T1>();
		}
	}
}