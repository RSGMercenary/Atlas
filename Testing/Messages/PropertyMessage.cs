namespace Atlas.Testing.Messages
{
	class PropertyMessage<TDispatcher, TProperty>:Message<TDispatcher, TProperty, TProperty>
	{
		public PropertyMessage()
		{

		}

		public PropertyMessage(string type, TDispatcher source, TDispatcher sender, TProperty next, TProperty previous)
		{
			Initialize(type, source, sender, next, previous);
		}

		public TProperty Next
		{
			get
			{
				return item1;
			}
		}

		public TProperty Previous
		{
			get
			{
				return item2;
			}
		}
	}
}
