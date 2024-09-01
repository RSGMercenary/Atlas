using Atlas.Core.Collections.LinkList;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Hierarchy;

public class Hierarchy<T> : IHierarchy<T>, IDisposable
		where T : class, IHierarchy<T>
{
	#region Events
	public event Action<T, T, T> RootChanged;
	public event Action<T, T, T> ParentChanged;
	public event Action<T, int, int> ParentIndexChanged;
	public event Action<T, T, int> ChildAdded;
	public event Action<T, T, int> ChildRemoved;
	public event Action<T> ChildrenChanged;
	#endregion

	#region Fields
	private readonly T Self;
	private T root;
	private T parent;
	private int parentIndex = -1;
	private readonly LinkList<T> children = new();
	#endregion

	#region Construct / Dispose
	public Hierarchy(T self = null)
	{
		Self = self ?? this as T;
	}

	public void Dispose()
	{
		RemoveChildren();
		Parent = null;
		IsRoot = false;

		RootChanged = null;
		ParentChanged = null;
		ParentIndexChanged = null;
		ChildAdded = null;
		ChildRemoved = null;
		ChildrenChanged = null;
	}
	#endregion

	#region Messages
	/*
	protected override Signal<TMessage> CreateSignal<TMessage>()
	{
		return new HierarchySignal<TMessage, T>();
	}

	public override void Message<TMessage>(TMessage message)
	{
		Message(message, Relation.All);
	}

	public void Message<TMessage>(TMessage message, Relation flow)
		where TMessage : IMessage<T>
	{
		//Keep track of what told 'this' to Message().
		//Prevents endless recursion.
		var previousMessenger = message.CurrentMessenger;

		//Standard Message() call.
		//Sets CurrentMessenger to 'this' and sends Message to TMessage listeners.
		base.Message(message);

		if(flow != Relation.All && root == this && flow.HasFlag(Relation.Root))
			return;

		if(flow == Relation.All || flow.HasFlag(Relation.Descendent) ||
			(flow.HasFlag(Relation.Child) && message.Messenger == this) ||
			(flow.HasFlag(Relation.Sibling) && HasChild(message.Messenger)))
		{
			//Send Message to children.
			foreach(var child in children)
			{
				//Don't send Message back to the child that told 'this' to Message().
				if(child == previousMessenger)
					continue;
				child.Message(message, flow);
				//Reset CurrentMessenger to 'this' so the next child (and parent)
				//can block messaging from 'this' parent messenger.
				(message as IMessage).CurrentMessenger = this;
			}
		}

		if(flow == Relation.All || (flow.HasFlag(Relation.Parent) && message.Messenger == this) ||
			(flow.HasFlag(Relation.Ancestor) && !HasSibling(previousMessenger)))
		{
			//Send Message to parent.
			//Don't send Message back to the parent that told 'this' to Message().
			if(parent != previousMessenger)
				parent?.Message(message, flow);
			(message as IMessage).CurrentMessenger = this;
		}

		//Send Message to siblings ONLY if the message flow wasn't going to get there eventually.
		if(flow != Relation.All && parent != null && flow.HasFlag(Relation.Sibling) &&
			message.Messenger == this)
		{
			foreach(var sibling in parent)
			{
				//Don't send Message back to the sibling that told 'this' to Message().
				if(sibling == this)
					continue;
				sibling.Message(message, flow);
				//Reset CurrentMessenger to 'this' so the next sibling
				//can block 'this' sibling messenger.
				(message as IMessage).CurrentMessenger = this;
			}
		}

		//Send Message to root ONLY if the message flow wasn't going to get there eventually.
		if(flow != Relation.All && root != null && flow.HasFlag(Relation.Root) &&
			message.Messenger == this && !flow.HasFlag(Relation.Ancestor) &&
			!(flow.HasFlag(Relation.Parent) && parent == root))
		{
			if(root != this)
				root?.Message(message, flow);
			(message as IMessage).CurrentMessenger = this;
		}
	}

	protected override void Messaging(IMessage<T> message)
	{
		if(message.Messenger == parent)
		{
			if(message is IRootMessage<T> rootMessage)
			{
				Root = rootMessage.Messenger.Root;
			}
			else if(message is IChildrenMessage<T>)
			{
				SetParentIndex(parent.GetChildIndex(this as T));
			}
		}
		base.Messaging(message);
	}

	public void AddListener<TMessage>(Action<TMessage> listener, Relation messenger)
		where TMessage : IMessage<T>
	{
		AddListener(listener, 0, messenger);
	}

	public void AddListener<TMessage>(Action<TMessage> listener, int priority, Relation messenger)
		where TMessage : IMessage<T>
	{
		(AddListenerSlot(listener, priority) as HierarchySlot<TMessage, T>).Messenger = messenger;
	}*/
	#endregion

	#region Root
	public T Root
	{
		get => root;
		private set
		{
			if(root == value)
				return;
			var previous = root;
			root = value;
			RootChanged?.Invoke(Self, value, previous);
		}
	}

	public bool IsRoot
	{
		get => Self == root;
		set
		{
			if(IsRoot == value)
				return;
			if(value)
			{
				Parent = null;
				Root = Self;
			}
			else
			{
				Root = null;
			}
		}
	}
	#endregion

	#region Parent
	public T Parent
	{
		get => parent;
		set => SetParent(value);
	}

	public int ParentIndex
	{
		get => parentIndex;
		set => parent?.SetChildIndex(Self, value);
	}

	private void SetParentIndex(int value)
	{
		if(parentIndex == value)
			return;
		int previous = parentIndex;
		parentIndex = value;
		ParentIndexChanged?.Invoke(Self, value, previous);
	}

	public T SetParent(T next) => SetParent(next, next?.Children.Count ?? -1);

	public T SetParent(T next, int index)
	{
		if(parent == next)
			return parent;
		//Prevent changing the Parent of the Root Entity. The root must be the bottom-most entity.
		//Prevent ancestor/descendant loops by blocking descendants becoming ancestors of their ancestors.
		if(IsRoot || Self == next || HasDescendant(next))
			throw new InvalidOperationException("Can't set the root's parent, or the parent to itself or a descendant.");
		var previous = parent;
		if(previous != null)
		{
			parent = null;
			previous.RootChanged -= OnParentRootChanged;
			previous.ChildrenChanged -= OnParentChildrenChanged;
			previous.RemoveChild(Self);
		}
		if(next != null)
		{
			parent = next;
			next.AddChild(Self, index);
			next.RootChanged += OnParentRootChanged;
			next.ChildrenChanged += OnParentChildrenChanged;
		}
		Root = next?.Root;
		ParentChanged?.Invoke(Self, next, previous);
		SetParentIndex(next != null ? index : -1);
		return next;
	}

	private void OnParentRootChanged(T parent, T current, T previous) => Root = current?.Root;

	private void OnParentChildrenChanged(T parent)
	{
		if(parent.GetChild(parentIndex) != Self)
			SetParentIndex(parent.GetChildIndex(Self));
	}
	#endregion

	#region Add
	public T AddChild(T child) => AddChild(child, children.Count);

	public T AddChild(T child, int index)
	{
		if(child.Parent == Self)
		{
			if(!HasChild(child))
			{
				children.Add(child, index);
				ChildAdded?.Invoke(Self, child, index);
				ChildrenChanged?.Invoke(Self);
			}
			else
			{
				SetChildIndex(child, index);
			}
		}
		else
		{
			child.SetParent(Self, index);
		}
		return child;
	}
	#endregion

	#region Remove
	public T RemoveChild(T child)
	{
		if(child.Parent != Self)
		{
			if(!HasChild(child))
				return null;
			int index = children.GetIndex(child);
			children.Remove(index);
			ChildRemoved?.Invoke(Self, child, index);
			ChildrenChanged?.Invoke(Self);
		}
		else
		{
			child.SetParent(null, -1);
		}
		return child;
	}

	public T RemoveChild(int index) => RemoveChild(children[index]);

	public bool RemoveChildren()
	{
		if(children.Count <= 0)
			return false;
		foreach(var child in children.Backward())
			child.Parent = null;
		return true;
	}
	#endregion

	#region Set
	public bool SetChildIndex(T child, int index)
	{
		var previous = children.Set(child, index);
		if(previous < 0)
			return false;
		if(previous != index)
			ChildrenChanged?.Invoke(Self);
		return true;
	}

	public bool SwapChildren(T child1, T child2)
	{
		if(!children.Swap(child1, child2))
			return false;
		if(child1 != child2)
			ChildrenChanged?.Invoke(Self);
		return true;
	}

	public bool SwapChildren(int index1, int index2)
	{
		if(!children.Swap(index1, index2))
			return false;
		if(index1 != index2)
			ChildrenChanged?.Invoke(Self);
		return true;
	}
	#endregion

	#region Get
	public IReadOnlyLinkList<T> Children => children;

	public T this[int index]
	{
		get => children[index];
		set
		{
			RemoveChild(index);
			AddChild(value, index);
		}
	}

	public T GetChild(int index)
	{
		if(index <= -1 || index >= children.Count)
			return null;
		return children[index];
	}

	public int GetChildIndex(T child) => children.GetIndex(child);

	public IEnumerator<T> GetEnumerator() => children.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	#region Has
	public bool HasDescendant(T descendant)
	{
		if(descendant == Self)
			return false;
		while(descendant != null && descendant != Self)
			descendant = descendant.Parent;
		return descendant == Self;
	}

	public bool HasAncestor(T ancestor) => ancestor?.HasDescendant(Self) ?? false;

	public bool HasChild(T child) => children.Contains(child);

	public bool HasSibling(T sibling) => (parent == null || sibling == null) ? false : parent == sibling.Parent;
	#endregion
}