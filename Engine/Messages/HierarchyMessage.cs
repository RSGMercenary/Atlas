namespace Atlas.Messages
{
	class HierarchyMessage<TSender>:Message<TSender>, IHierarchyMessage<TSender>
	{
		private TSender origin;

		public HierarchyMessage()
		{

		}

		public HierarchyMessage(string type, TSender origin, TSender sender)
		{
			Initialize(type, origin, sender);
		}

		override public void Dispose()
		{
			origin = default(TSender);
			base.Dispose();
		}

		public void Initialize(string type, TSender origin, TSender sender)
		{
			Initialize(type, sender);
			this.origin = origin;
		}

		public TSender Origin
		{
			get
			{
				return origin;
			}
			private set
			{
				origin = value;
			}
		}
	}
}
