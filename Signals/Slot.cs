using System;

namespace Atlas.Signals
{
    sealed class Slot<T1, T2> where T1 : class
    {
        private Signal<T1, T2> signal;
        private Action<T1, T2> listener;
        private int priority = 0;
        private bool isOnce = false;
        private bool isEnabled = true;

        internal Slot()
        {
            
        }

        public void Dispose()
	    {
		    if(signal != null)
		    {
			    signal.Remove(listener);
		    }
		    else
		    {
                signal = null;
                listener = null;
                priority = 0;
                isOnce = false;
			    isEnabled = true;
            }
        }

        public Signal<T1, T2> Signal
        {
            get
            {
                return signal;
            }
            internal set
            {
                signal = value;
            }
        }

        public Action<T1, T2> Listener
        {
            get
            {
                return listener;
            }
            internal set
            {
                listener = value;
            }
        }

        public bool IsOnce
        {
            get
            {
                return isOnce;
            }
            set
            {
                isOnce = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }

        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                if(priority != value)
                {
                    int previous = priority;
                    priority = value;
                    if(signal != null)
                    {
                        signal.PriorityChanged(this, previous);
                    }
                }
            }
        }
    }
}
