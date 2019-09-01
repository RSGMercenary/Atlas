using Atlas.Core.Collections.Group;
using Atlas.Core.Signals;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.Core.Messages
{
	public class HierarchyMessenger<T> : Messenger<T>, IHierarchyMessenger<T>
		where T : class, IHierarchyMessenger<T>
	{
		private T root;
		private T parent;
		private int parentIndex = -1;
		private readonly Group<T> children = new Group<T>();

		public HierarchyMessenger(T target, Action<IMessage<T>> callout = null) : base(target, callout) { }

		public override void Dispose()
		{
			RemoveChildren();
			Parent = null;
			Root = null;
			base.Dispose();
		}

		#region Messages
		protected override Signal<TMessage> CreateSignal<TMessage>()
		{
			return new HierarchySignal<TMessage, T>();
		}

		public override void Message<TMessage>(TMessage message)
		{
			Message(message, MessageFlow.All);
		}

		public void Message<TMessage>(TMessage message, MessageFlow flow)
			where TMessage : IMessage<T>
		{
			//Keep track of what told 'this' to Message().
			//Prevents endless recursion.
			var previousMessenger = message.CurrentMessenger;

			//Standard Message() call.
			//Sets CurrentMessenger to 'this' and sends Message to TMessage listeners.
			base.Message(message);

			if(flow != MessageFlow.All && root == Target && flow.HasFlag(MessageFlow.Root))
				return;

			if(flow == MessageFlow.All || flow.HasFlag(MessageFlow.Descendent) ||
				(flow.HasFlag(MessageFlow.Child) && message.Messenger == Target) ||
				(flow.HasFlag(MessageFlow.Sibling) && HasChild(message.Messenger)))
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
					(message as IMessage).CurrentMessenger = Target;
				}
			}

			if(flow == MessageFlow.All || (flow.HasFlag(MessageFlow.Parent) && message.Messenger == Target) ||
				(flow.HasFlag(MessageFlow.Ancestor) && !HasSibling(previousMessenger)))
			{
				//Send Message to parent.
				//Don't send Message back to the parent that told 'this' to Message().
				if(parent != previousMessenger)
					parent?.Message(message, flow);
				(message as IMessage).CurrentMessenger = Target;
			}

			//Send Message to siblings ONLY if the message flow wasn't going to get there eventually.
			if(flow != MessageFlow.All && parent != null && flow.HasFlag(MessageFlow.Sibling) &&
				message.Messenger == Target)
			{
				foreach(var sibling in parent)
				{
					//Don't send Message back to the sibling that told 'this' to Message().
					if(sibling == Target)
						continue;
					sibling.Message(message, flow);
					//Reset CurrentMessenger to 'this' so the next sibling
					//can block 'this' sibling messenger.
					(message as IMessage).CurrentMessenger = Target;
				}
			}

			//Send Message to root ONLY if the message flow wasn't going to get there eventually.
			if(flow != MessageFlow.All && root != null && flow.HasFlag(MessageFlow.Root) &&
				message.Messenger == Target && !flow.HasFlag(MessageFlow.Ancestor) &&
				!(flow.HasFlag(MessageFlow.Parent) && parent == root))
			{
				if(root != Target)
					root?.Message(message, flow);
				(message as IMessage).CurrentMessenger = Target;
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
					SetParentIndex(parent.GetChildIndex(Target));
				}
			}
			base.Messaging(message);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, MessageFlow messenger)
			where TMessage : IMessage<T>
		{
			AddListener(listener, 0, messenger);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority, MessageFlow messenger)
			where TMessage : IMessage<T>
		{
			(AddListenerSlot(listener, priority) as HierarchySlot<TMessage, T>).Messenger = messenger;
		}
		#endregion

		#region Root
		public T Root
		{
			get { return root; }
			private set
			{
				if(root == value)
					return;
				var previous = root;
				root = value;
				Message<IRootMessage<T>>(new RootMessage<T>(value, previous));
			}
		}

		public bool IsRoot
		{
			get { return Target == root; }
			set
			{
				if(IsRoot == value)
					return;
				if(value)
				{
					Parent = null;
					Root = Target;
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
			get { return parent; }
			set { SetParent(value); }
		}

		public int ParentIndex
		{
			get { return parentIndex; }
			set { parent?.SetChildIndex(Target, value); }
		}

		public T SetParent(T next, int index = int.MaxValue)
		{
			//Prevent changing the Parent of the Root Entity. The root must be the bottom-most entity.
			//Prevent ancestor/descendant loops by blocking descendants becoming ancestors of their ancestors.
			if(Target == root || Target == next || HasDescendant(next))
				return null;
			Root = next?.Root;
			var previous = parent;
			//TO-DO This may need more checking if parent multi-setting happens during Dispatches.
			if(previous != null && previous != next)
			{
				parent = null;
				previous.RemoveChild(Target);
			}
			if(next != null)
			{
				parent = next;
				index = Math.Max(0, Math.Min(index, next.Children.Count));
				next.AddChild(Target, index);
			}
			if(previous != next)
			{
				Message<IParentMessage<T>>(new ParentMessage<T>(next, previous));
				SetParentIndex(next != null ? index : -1);
			}
			return next;
		}

		private void SetParentIndex(int value)
		{
			if(parentIndex == value)
				return;
			int previous = parentIndex;
			parentIndex = value;
			Message<IParentIndexMessage<T>>(new ParentIndexMessage<T>(value, previous));
		}
		#endregion

		#region Add
		public T AddChild(T child)
		{
			return AddChild(child, children.Count);
		}

		public T AddChild(T child, int index)
		{
			if(child?.Parent == Target)
			{
				if(!HasChild(child))
				{
					children.Insert(index, child);
					Message<IChildAddMessage<T>>(new ChildAddMessage<T>(index, child));
					Message<IChildrenMessage<T>>(new ChildrenMessage<T>());
				}
				else
				{
					SetChildIndex(child, index);
				}
			}
			else
			{
				if(child?.SetParent(Target, index) == null)
					return null;
			}
			return child;
		}
		#endregion

		#region Remove
		public T RemoveChild(T child)
		{
			if(child == null)
				return null;
			if(child.Parent != Target)
			{
				if(!HasChild(child))
					return null;
				int index = children.IndexOf(child);
				children.RemoveAt(index);
				Message<IChildRemoveMessage<T>>(new ChildRemoveMessage<T>(index, child));
				Message<IChildrenMessage<T>>(new ChildrenMessage<T>());
			}
			else
			{
				child.SetParent(null, -1);
			}
			return child;
		}

		public T RemoveChild(int index)
		{
			return RemoveChild(children[index]);
		}

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
			int previous = children.IndexOf(child);

			if(previous == index || previous < 0)
				return false;

			index = Math.Max(0, Math.Min(index, children.Count - 1));

			children.RemoveAt(previous);
			children.Insert(index, child);
			Message<IChildrenMessage<T>>(new ChildrenMessage<T>());
			return true;
		}

		public bool SwapChildren(T child1, T child2)
		{
			if(child1 == null || child2 == null)
				return false;
			int index1 = children.IndexOf(child1);
			int index2 = children.IndexOf(child2);
			return SwapChildren(index1, index2);
		}

		public bool SwapChildren(int index1, int index2)
		{
			if(!children.Swap(index1, index2))
				return false;
			Message<IChildrenMessage<T>>(new ChildrenMessage<T>());
			return true;
		}
		#endregion

		#region Get
		public IReadOnlyGroup<T> Children => children;

		public T GetChild(int index)
		{
			return children[index];
		}

		public int GetChildIndex(T child)
		{
			return children.IndexOf(child);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		#region Has
		public bool HasDescendant(T descendant)
		{
			if(descendant == Target)
				return false;
			while(descendant != null && descendant != Target)
				descendant = descendant.Parent;
			return descendant == Target;
		}

		public bool HasAncestor(T ancestor)
		{
			return ancestor?.HasDescendant(Target) ?? false;
		}

		public bool HasChild(T child)
		{
			return children.Contains(child);
		}

		public bool HasSibling(T sibling)
		{
			if(parent == null)
				return false;
			foreach(var child in parent)
			{
				if(child == Target)
					continue;
				if(child == sibling)
					return true;
			}
			return false;
		}
		#endregion
	}
}