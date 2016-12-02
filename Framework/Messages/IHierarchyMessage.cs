namespace Atlas.Messages
{
	interface IHierarchyMessage<TSender>:IMessage<TSender>
	{
		TSender Origin { get; }
	}
}
