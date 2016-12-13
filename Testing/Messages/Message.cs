using System;

namespace Atlas.Testing.Messages
{
	class Message:IMessage, IDisposable
	{
		protected string type = "";
		protected object source;
		protected object sender;

		public Message()
		{

		}

		public Message(string type = "", object source = null, object sender = null)
		{
			Initialize(type, source, sender);
		}

		virtual public void Initialize(string type, object source, object sender)
		{
			this.type = type;
			this.source = source;
			this.sender = sender;
		}

		virtual public void Dispose()
		{
			type = "";
			sender = null;
		}

		public object Source
		{
			get
			{
				return source;
			}
		}

		public object Sender
		{
			get
			{
				return sender;
			}
		}

		public string Type
		{
			get
			{
				return type;
			}
		}
	}

	class Message<TDispatcher>:Message, IMessage<TDispatcher>
	{
		public Message()
		{

		}

		public Message(string type, TDispatcher source, TDispatcher sender)
		{
			base.Initialize(type, source, sender);
		}

		public new TDispatcher Source
		{
			get
			{
				return (TDispatcher)source;
			}
		}

		public new TDispatcher Sender
		{
			get
			{
				return (TDispatcher)sender;
			}
		}

		virtual public void Initialize(string type, TDispatcher source, TDispatcher sender)
		{
			base.Initialize(type, source, sender);
		}
	}

	class Message<TDispatcher, T1>:Message<TDispatcher>, IMessage<TDispatcher, T1>
	{
		protected T1 item1;

		public Message()
		{

		}

		public Message(string type, TDispatcher source, TDispatcher sender, T1 item1)
		{
			Initialize(type, source, sender, item1);
		}

		virtual public void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1)
		{
			Initialize(type, source, sender);
			this.item1 = item1;
		}
	}

	class Message<TDispatcher, T1, T2>:Message<TDispatcher, T1>, IMessage<TDispatcher, T1, T2>
	{
		protected T2 item2;

		public Message()
		{

		}

		public Message(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2)
		{
			Initialize(type, source, sender, item1, item2);
		}

		virtual public void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2)
		{
			Initialize(type, source, sender, item1);
			this.item2 = item2;
		}
	}

	class Message<TDispatcher, T1, T2, T3>:Message<TDispatcher, T1, T2>, IMessage<TDispatcher, T1, T2, T3>
	{
		protected T3 item3;

		public Message()
		{

		}

		public Message(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2, T3 item3)
		{
			Initialize(type, source, sender, item1, item2, item3);
		}

		virtual public void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2, T3 item3)
		{
			Initialize(type, source, sender, item1, item2);
			this.item3 = item3;
		}
	}

	class Message<TDispatcher, T1, T2, T3, T4>:Message<TDispatcher, T1, T2, T3>, IMessage<TDispatcher, T1, T2, T3, T4>
	{
		protected T4 item4;

		public Message()
		{

		}

		public Message(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2, T3 item3, T4 item4)
		{
			Initialize(type, source, sender, item1, item2, item3, item4);
		}

		virtual public void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2, T3 item3, T4 item4)
		{
			Initialize(type, source, sender, item1, item2, item3);
			this.item4 = item4;
		}
	}
}
