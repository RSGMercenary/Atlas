using System;
using System.Collections.Generic;

namespace Atlas.Signals
{
    sealed class Signal<T1, T2> where T1 : class
    {
        private List<Slot<T1, T2>> slots = new List<Slot<T1, T2>>();
        private List<Slot<T1, T2>> slotsPooled = new List<Slot<T1, T2>>();
        private List<Slot<T1, T2>> slotsRemoved = new List<Slot<T1, T2>>();

        private int numDispatches = 0;

        public Signal()
        {
            
        }

        public void Dispose()
        {
            RemoveAll();
            while(slotsPooled.Count > 0)
            {
                Slot<T1, T2> slot = slotsPooled[slotsPooled.Count - 1];
                slotsPooled.RemoveAt(slotsPooled.Count - 1);
                slot.Dispose();
            }
        }

        public int NumDispatches
        {
            get
            {
                return numDispatches;
            }
        }

        public int NumSlots
        {
            get
            {
                return slots.Count;
            }
        }

        public Slot<T1, T2>[] Slots
        {
            get
            {
                return slots.ToArray();
            }
        }
        
        public void Dispatch(T1 item1, T2 item2)
        {
            if(slots.Count > 0)
            {
                ++numDispatches;

                //We have to clone the slots so this dispatch() has its own unique list.
                List<Slot<T1, T2>> slotsDispatch = new List<Slot<T1, T2>>(slots);
               
                foreach(var slot in slotsDispatch)
                {
                    if(slot.IsEnabled)
                    {
                        //If this listener is only being called once...
                        if (slot.IsOnce)
                        {
                            //...we need to remove it from the base slots so it won't be called again on the next dispatch.
                            Remove(slot.Listener);
                        }

                        //Set the passed in args...
                        /*var allArgs:Array < Dynamic > = args;
                        //...and then add the slot's personal args to it.
                        if (slot._args != null && slot._args.length > 0)
                        {
                            allArgs = allArgs.concat(slot._args);
                        }*/

                        try
                        {
                            slot.Listener.Invoke(item1, item2);
                        }
                        catch(Exception e)
                        {
                            //We remove the Slot so the Error doesn't inevitably happen again.
                            Remove(slot.Listener);
                            //Console.WriteLine(e.StackTrace);
                        }
                    }
                }
			
			    --numDispatches;
			
			    if(numDispatches == 0)
			    {
				    while(slotsRemoved.Count > 0)
				    {
                        var slot = slotsRemoved[slotsRemoved.Count - 1];
                        slotsRemoved.RemoveAt(slotsRemoved.Count);
					    DisposeSlot(slot);
				    }
                }
		    }
        }

        private void DisposeSlot(Slot<T1, T2> slot)
	    {
		    slot.Signal = null;
		    slot.Dispose();
		    slotsPooled.Add(slot);
	    }

        public Slot<T1, T2> Get(Action<T1, T2> listener)
	    {
		    foreach(var slot in slots)
		    {
			    if(slot.Listener == listener)
			    {
				    return slot;
			    }
            }
		    return null;
	    }

        public Slot<T1, T2> GetAt(int index)
	    {
		    if(index < 0) 				return null;
		    if(index > slots.Count - 1) return null;
		    return slots[index];
	    }

        public int GetIndex(Action<T1, T2> listener)
	    {
		    for(int index = slots.Count - 1; index > -1; --index)
		    {
			    if(slots[index].Listener == listener)
			    {
				    return index;
			    }
            }
		    return -1;
	    }

        public Slot<T1, T2> Add(Action<T1, T2> listener)
        {
            return Add(listener, 0, false);
        }

        public Slot<T1, T2> Add(Action<T1, T2> listener, int priority = 0)
        {
            return Add(listener, priority, false);
        }

        public Slot<T1, T2> AddOnce(Action<T1, T2> listener, int priority = 0)
        {
            return Add(listener, priority, true);
        }

        public Slot<T1, T2> Add(Action<T1, T2> listener, int priority = 0, bool isOnce = false)
	    {
		    if(listener != null)
		    {
			    Slot<T1, T2> slot = Get(listener);
			    if(slot == null)
			    {
				    if(slotsPooled.Count > 0)
				    {
                        slot = slots[slotsPooled.Count - 1];
					    slotsPooled.RemoveAt(slotsPooled.Count - 1);
				    }
				    else
				    {
					    slot = new Slot<T1, T2>();
                    }

                    slot.Signal = this;
				    slot.Listener = listener;
                    slot.Priority = priority;
                    slot.IsOnce = isOnce;
                    
				    PriorityChanged(slot, 0);
				
				    return slot;
                }    
		    }
		    return null;
	    }

        internal void PriorityChanged(Slot<T1, T2> slot, int previousPriority)
	    {
            slots.Remove(slot);
		   
            for(int index = slots.Count; index > 0; --index)
            {
                if(slots[index - 1].Priority <= slot.Priority)
                {
                    slots.Insert(index, slot);
                    return;
                }
            }

            slots.Add(slot);
        }

        public Slot<T1, T2> Remove(Action<T1, T2> listener)
        {
            for(int index = slots.Count - 1; index > -1; --index)
            {
                if(slots[index].Listener == listener)
                {
                    return RemoveAt(index);
                }
            }
            return null;
        }

        public Slot<T1, T2> RemoveAt(int index)
        {
            Slot<T1, T2> slot = slots[index];
            slots.RemoveAt(index);

            if(numDispatches > 0)
            {
                slotsRemoved.Add(slot);
            }
            else
            {
                DisposeSlot(slot);
            }
            return slot;
        }

        public void RemoveAll()
        {
            while(slots.Count > 0)
            {
                RemoveAt(slots.Count - 1);
            }
        }
    }
}
