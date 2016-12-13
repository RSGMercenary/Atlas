using System;
using System.Diagnostics;

namespace Atlas.Engine.Signals
{
	class SignalDynamic:SignalBase<ISlotBase, Delegate>, ISignalDynamic
	{
		public SignalDynamic()
		{

		}

		public bool Dispatch(params object[] items)
		{
			if(DispatchStart())
			{
				foreach(SlotBase slot in SlotsCopy)
				{
					try
					{
						slot.Listener.DynamicInvoke(items);
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
	}
}