using System;

namespace Atlas.Messages
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

		public void Initialize(string type, TSender sender)
		{
			base.Initialize(type, sender);
		}
	}
}
