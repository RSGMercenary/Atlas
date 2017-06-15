namespace Atlas.Testing.Messages
{
	class Message : IMessage
	{
		protected bool isInitialized = false;

		public Message()
		{

		}

		public virtual void Initialize()
		{

		}

		public virtual void Dispose()
		{

		}
	}

	class Message<T1> : Message, IMessage<T1>
	{
		private T1 item1;

		public Message()
		{

		}

		public Message(T1 item1)
		{
			Initialize(item1);
		}

		public void Initialize(T1 item1)
		{
			Initialize();
			this.item1 = item1;
		}

		public T1 Item1
		{
			get
			{
				return item1;
			}
		}
	}

	class Message<T1, T2> : Message<T1>, IMessage<T1, T2>
	{
		private T2 item2;

		public Message()
		{

		}

		public Message(T1 item1, T2 item2)
		{
			Initialize(item1, item2);
		}

		public void Initialize(T1 item1, T2 item2)
		{
			Initialize(item1);
			this.item2 = item2;
		}

		public T2 Item2
		{
			get
			{
				return item2;
			}
		}
	}

	class Message<T1, T2, T3> : Message<T1, T2>, IMessage<T1, T2, T3>
	{
		private T3 item3;

		public Message()
		{

		}

		public Message(T1 item1, T2 item2, T3 item3)
		{
			Initialize(item1, item2, item3);
		}

		public void Initialize(T1 item1, T2 item2, T3 item3)
		{
			Initialize(item1, item2);
			this.item3 = item3;
		}

		public T3 Item3
		{
			get
			{
				return item3;
			}
		}
	}

	class Message<T1, T2, T3, T4> : Message<T1, T2, T3>, IMessage<T1, T2, T3, T4>
	{
		private T4 item4;

		public Message()
		{

		}

		public Message(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			Initialize(item1, item2, item3, item4);
		}

		public void Initialize(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			Initialize(item1, item2, item3);
			this.item4 = item4;
		}

		public T4 Item4
		{
			get
			{
				return item4;
			}
		}
	}

	class Message<T1, T2, T3, T4, T5> : Message<T1, T2, T3, T4>, IMessage<T1, T2, T3, T4, T5>
	{
		private T5 item5;

		public Message()
		{

		}

		public Message(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			Initialize(item1, item2, item3, item4, item5);
		}

		public void Initialize(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			Initialize(item1, item2, item3, item4);
			this.item5 = item5;
		}

		public T5 Item5
		{
			get
			{
				return item5;
			}
		}
	}
}
