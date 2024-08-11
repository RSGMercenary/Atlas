using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy;

#region Interfaces

public interface IChildAddMessage<out T> : IKeyValueMessage<T, int, T> where T : IMessenger, IHierarchy<T> { }

public interface IChildRemoveMessage<out T> : IKeyValueMessage<T, int, T> where T : IMessenger, IHierarchy<T> { }

public interface IChildrenMessage<out T> : IMessage<T> where T : IMessenger, IHierarchy<T> { }

public interface IParentMessage<out T> : IPropertyMessage<T, T> where T : IMessenger, IHierarchy<T> { }

public interface IParentIndexMessage<out T> : IPropertyMessage<T, int> where T : IMessenger, IHierarchy<T> { }

public interface IRootMessage<out T> : IPropertyMessage<T, T> where T : IMessenger, IHierarchy<T> { }

#endregion

#region Classes

class ChildAddMessage<T> : KeyValueMessage<T, int, T>, IChildAddMessage<T> where T : IMessenger, IHierarchy<T>
{
	public ChildAddMessage(int key, T value) : base(key, value) { }
}

class ChildRemoveMessage<T> : KeyValueMessage<T, int, T>, IChildRemoveMessage<T> where T : IMessenger, IHierarchy<T>
{
	public ChildRemoveMessage(int key, T value) : base(key, value) { }
}

class ChildrenMessage<T> : Message<T>, IChildrenMessage<T> where T : IMessenger, IHierarchy<T>
{
}

class ParentMessage<T> : PropertyMessage<T, T>, IParentMessage<T> where T : IMessenger, IHierarchy<T>
{
	public ParentMessage(T current, T previous) : base(current, previous) { }
}

class ParentIndexMessage<T> : PropertyMessage<T, int>, IParentIndexMessage<T> where T : IMessenger, IHierarchy<T>
{
	public ParentIndexMessage(int current, int previous) : base(current, previous) { }
}

class RootMessage<T> : PropertyMessage<T, T>, IRootMessage<T> where T : IMessenger, IHierarchy<T>
{
	public RootMessage(T current, T previous) : base(current, previous) { }
}

#endregion