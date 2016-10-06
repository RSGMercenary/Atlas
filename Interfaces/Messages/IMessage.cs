namespace Atlas.Interfaces.Messages
{
	interface IMessage<Sender> where Sender : class
	{
		string Type
		{
			get;
		}

		Sender First
		{
			get;
		}

		Sender Current
		{
			get;
		}
	}
}
