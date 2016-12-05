namespace Atlas.Testing.Messages
{
	interface IHierarchyMessage<TSender>:IMessage<TSender>
	{
		TSender Origin { get; }
	}
}
