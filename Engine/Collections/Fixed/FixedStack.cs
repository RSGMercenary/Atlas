using System.Collections.Generic;

namespace Atlas.Engine.Collections.Fixed
{
	public class FixedStack<T> : Stack<T>
	{
		private int capacity = 0;

		public FixedStack() : this(0)
		{

		}

		public FixedStack(int capacity) : base(capacity)
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
						Pop();
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

		public new void Push(T item)
		{
			if(IsFixed && Count == capacity)
				return;
			base.Push(item);
		}
	}
}
