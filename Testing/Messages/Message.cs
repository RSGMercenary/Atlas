using System;

namespace Atlas.Testing.Messages
{
	class Message:IMessage, IDisposable
	{
		protected string type = "";
		protected object sender;

		public Message()
		{

		}

		public Message(string type = "", object sender = null)
		{
			this.type = type;
			this.sender = sender;
		}

		virtual public void Initialize(string type, object sender)
		{
			this.type = type;
			this.sender = sender;
		}

		virtual public void Dispose()
		{
			type = "";
			sender = null;
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

	class Message<TSender>:Message, IMessage<TSender>
	{
		public Message()
		{

		}

		public Message(string type, TSender sender)
		{
			Initialize(type, sender);
		}

		public new TSender Sender
		{
			get
			{
				return (TSender)sender;
			}
		}

		virtual public void Initialize(string type, TSender sender)
		{
			base.Initialize(type, sender);
		}
	}

	class Message<TSender, T1>:Message<TSender>, IMessage<TSender, T1>
	{
		public Message()
		{

		}

		public Message(string type, TSender sender, T1 item1)
		{
			Initialize(type, sender, item1);
		}

		virtual public void Initialize(string type, TSender sender, T1 item1)
		{
			Initialize(type, sender);
		}
	}

	class Message<TSender, T1, T2>:Message<TSender, T1>, IMessage<TSender, T1, T2>
	{
		public Message()
		{

		}

		public Message(string type, TSender sender, T1 item1, T2 item2)
		{
			Initialize(type, sender, item1, item2);
		}

		virtual public void Initialize(string type, TSender sender, T1 item1, T2 item2)
		{
			Initialize(type, sender, item1);
		}
	}

	class Message<TSender, T1, T2, T3>:Message<TSender, T1, T2>, IMessage<TSender, T1, T2, T3>
	{
		public Message()
		{

		}

		public Message(string type, TSender sender, T1 item1, T2 item2, T3 item3)
		{
			Initialize(type, sender, item1, item2, item3);
		}

		virtual public void Initialize(string type, TSender sender, T1 item1, T2 item2, T3 item3)
		{
			Initialize(type, sender, item1, item2);
		}
	}

	class Message<TSender, T1, T2, T3, T4>:Message<TSender, T1, T2, T3>, IMessage<TSender, T1, T2, T3, T4>
	{
		public Message()
		{

		}

		public Message(string type, TSender sender, T1 item1, T2 item2, T3 item3, T4 item4)
		{
			Initialize(type, sender, item1, item2, item3, item4);
		}

		virtual public void Initialize(string type, TSender sender, T1 item1, T2 item2, T3 item3, T4 item4)
		{
			Initialize(type, sender, item1, item2, item3);
		}
	}
}
