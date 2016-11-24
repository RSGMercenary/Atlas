namespace Atlas.Signals
{
	class SignalDynamic:SignalBase, ISignalDynamic
	{
		public SignalDynamic()
		{

		}

		public void Dispatch(params object[] items)
		{
			if(DispatchStart())
			{
				foreach(SlotBase slot in Slots)
				{
					try
					{
						slot.Listener.DynamicInvoke(items);
					}
					catch
					{
						//We remove the Slot so the Error doesn't inevitably happen again.
						Remove(slot.Listener);
					}
				}
				DispatchStop();
			}
		}
	}
}