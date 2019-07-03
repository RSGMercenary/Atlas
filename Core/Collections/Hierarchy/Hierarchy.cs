namespace Atlas.Core.Collections.Hierarchy
{
	/*
	public class Hierarchy<T> : IHierarchy<T>
		where T : class, IHierarchy<T>, IMessenger<T>
	{
		private T root;
		private T parent;
		private int parentIndex = -1;
		private readonly Group<T> children = new Group<T>();

		private T target;

		public Hierarchy(T target)
		{
			this.target = target;
		}

		public T Parent
		{
			get { return parent; }
			set { SetParent(value); }
		}

		public int ParentIndex
		{
			get { return parentIndex; }
			set { parent?.SetChildIndex(target, value); }
		}

		public T SetParent(T next, int index = int.MaxValue)
		{
			//Prevent changing the Parent of the Root Entity.
			//The Root must be the absolute bottom of the hierarchy.
			if(this == singleton)
				return null;
			if(parent == next)
				return null;
			//Can't set a descendant of this as a parent.
			if(HasDescendant(next))
				return null;
			Root = next?.Root;
			var previous = parent;
			int sleeping = 0;
			//TO-DO This may need more checking if parent multi-setting happens during Dispatches.
			if(previous != null)
			{
				parent = null;
				previous.RemoveChild(target);
				//if(!IsFreeSleeping && previous.IsSleeping)
				//--sleeping;
			}
			if(next != null)
			{
				parent = next;
				index = Math.Max(0, Math.Min(index, next.Children.Count));
				next.AddChild(target, index);
				//if(!IsFreeSleeping && next.IsSleeping)
				//++sleeping;
			}
			target.Message<IParentMessage>(new ParentMessage(next, previous));
			SetParentIndex(next != null ? index : -1);
			Sleeping += sleeping;
			//if(autoDispose && parent == null)
			//Dispose();
			return next;
		}

		public T AddChild(T child)
		{
			return AddChild(child, children.Count);
		}

		public T AddChild(T child, int index)
		{
			if(child?.Parent == this)
			{
				if(!HasChild(child))
				{
					//if(HasChild(child.LocalName))
					//child.LocalName = UniqueName;
					children.Insert(index, child);
					target.Message<IChildAddMessage>(new ChildAddMessage(index, child));
					target.Message<IChildrenMessage>(new ChildrenMessage());
				}
				else
				{
					SetChildIndex(child, index);
				}
			}
			else
			{
				if(child?.SetParent(target, index) == null)
					return null;
			}
			return child;
		}

		public T RemoveChild(T child)
		{
			if(child == null)
				return null;
			if(child.Parent != this)
			{
				if(!HasChild(child))
					return null;
				int index = children.IndexOf(child);
				children.Remove(child);
				target.Message<IChildRemoveMessage>(new ChildRemoveMessage(index, child));
				target.Message<IChildrenMessage>(new ChildrenMessage());
			}
			else
			{
				child.SetParent(null, -1);
			}
			return child;
		}

		public T RemoveChild(int index)
		{
			throw new System.NotImplementedException();
		}

		public bool RemoveChildren()
		{
			throw new System.NotImplementedException();
		}

		public bool SetChildIndex(T child, int index)
		{
			throw new System.NotImplementedException();
		}

		public bool SwapChildren(T child1, T child2)
		{
			throw new System.NotImplementedException();
		}

		public bool SwapChildren(int index1, int index2)
		{
			throw new System.NotImplementedException();
		}

		public T Root
		{
			get { return root; }
			private set
			{
				//Only need this if Root setter becomes public
				//if(Parent?.Root != value)
					//return;
				if(root == value)
					return;
				var previous = root;
				root = value;
				target.Message<IRootMessage>(new RootMessage(value, previous));
			}
		}

		public IReadOnlyGroup<T> Children => throw new System.NotImplementedException();

		public T GetChild(int index)
		{
			throw new System.NotImplementedException();
		}

		public int GetChildIndex(T child)
		{
			throw new System.NotImplementedException();
		}

		public bool HasDescendant(T descendant)
		{
			throw new System.NotImplementedException();
		}

		public bool HasAncestor(T ancestor)
		{
			throw new System.NotImplementedException();
		}

		public bool HasChild(T child)
		{
			throw new System.NotImplementedException();
		}

		public bool HasSibling(T sibling)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new System.NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new System.NotImplementedException();
		}
	}
	*/
}
