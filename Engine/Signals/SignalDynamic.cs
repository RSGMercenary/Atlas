namespace Atlas.Engine.Signals
{
	class SignalDynamic:SignalBase, ISignalDynamic
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
					catch
					{
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