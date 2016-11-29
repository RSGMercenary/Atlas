using System.Collections.Generic;

namespace Atlas.Engine.Collections.Fixed
{
	class FixedQueue<T>:Queue<T>
	{
		private int capacity = 0;

		public FixedQueue() : this(0)
		{

		}

		public FixedQueue(int capacity = 0) : base(capacity)
		{
			Capacity = capacity;
		}

		public int Capacity
		{
			get
			{
				return capacity;
			}
			set
			{
				if(capacity == value)
					return;
				capacity = value;
				if(IsFixed)
				{
					while(Count > capacity)
					{
						Dequeue();
					}
				}
			}
		}

		bool IsFixed
		{
			get
			{
				return capacity > 0;
			}
		}

		public new void Enqueue(T item)
		{
			if(IsFixed && Count == capacity)
				Dequeue();
			base.Enqueue(item);
		}
	}
}
