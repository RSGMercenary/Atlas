namespace Atlas.Engine.Messages
{
	public class Message
	{
		private object target;
		private object currentTarget;

		public object Target
		{
			get { return target; }
			set { target = target == null ? value : target; }
		}

		public object CurrentTarget
		{
			get { return currentTarget; }
			set { currentTarget = value; }
		}

		public bool AtTarget
		{
			get { return (bool)target?.Equals(currentTarget); }
		}
	}

	public class Message<TSender> : Message, IMessage<TSender>
	{
		public Message()
		{

		}

		new public TSender Target
		{
			get { return (TSender)base.Target; }
			set { base.Target = value; }
		}
	}
}