using Atlas.Core.Collections.EngineList;
using System;

namespace Atlas.Core.Collections.Hierarchy
{
	public class Hierarchy : IHierarchy
	{
		private IReadOnlyHierarchy root;
		private IReadOnlyHierarchy parent;
		private int parentIndex = -1;
		private EngineList<IReadOnlyHierarchy> children = new EngineList<IReadOnlyHierarchy>();

		IReadOnlyHierarchy IReadOnlyHierarchy.Root
		{
			get { return root; }
		}

		public IHierarchy Root
		{
			get { return (IHierarchy)root; }
			protected set
			{
				if(root == value)
					return;
				var previous = root;
				root = value;
				//Message<IRootMessage>(new RootMessage(this, value, previous));
			}
		}

		IReadOnlyHierarchy IReadOnlyHierarchy.Parent
		{
			get { return parent; }
		}

		public IHierarchy Parent
		{
			get { return (IHierarchy)parent; }
			set { SetParent(value, int.MaxValue); }
		}

		public bool HasChild(IReadOnlyHierarchy child)
		{
			return children.Contains(child);
		}

		public bool AddChildren(int index, params IHierarchy[] children)
		{
			bool success = true;
			foreach(var child in children)
			{
				if(AddChild(child, index++) == null)
					success = false;
			}
			return success;
		}

		public bool AddChildren(params IHierarchy[] children)
		{
			return AddChildren(this.children.Count, children);
		}

		public IHierarchy AddChild(IHierarchy child)
		{
			return AddChild(child, children.Count);
		}

		public IHierarchy AddChild(IHierarchy child, int index)
		{
			if(child == null)
				return null;
			if(child.Parent == this)
			{
				if(!HasChild(child))
				{
					//if(HasChild(child.LocalName))
					//child.LocalName = UniqueName;
					children.Insert(index, child);
					//Message<IChildAddMessage>(new ChildAddMessage(this, index, child));
					//Message<IChildrenMessage>(new ChildrenMessage(this));
				}
				else
				{
					SetChildIndex(child, index);
				}
			}
			else
			{
				if(!child.SetParent(this, index))
					return null;
			}
			return child;
		}

		public IHierarchy RemoveChild(IHierarchy child)
		{
			if(child == null)
				return null;
			if(child.Parent != this)
			{
				if(!HasChild(child))
					return null;
				int index = children.IndexOf(child);
				//Message<IChildRemoveMessage>(new ChildRemoveMessage(this, index, child));
				//Message<IChildrenMessage>(new ChildrenMessage(this));
				//Could've been readded during messaging?
				if(child.Parent != this)
					children.Remove(child);
			}
			else
			{
				child.SetParent(null, -1);
			}
			return child;
		}

		public IHierarchy RemoveChild(int index)
		{
			return RemoveChild((IHierarchy)children[index]);
		}

		public bool RemoveChildren()
		{
			if(children.Count <= 0)
				return false;
			//foreach(var child in children.Backward())
			//child.Dispose();
			return true;
		}

		public bool SetParent(IHierarchy parent, int index)
		{
			//Prevent changing the Parent of the Root Entity.
			//The Root must be the absolute bottom of the hierarchy.
			if(this == Root)
				return false;
			if(this.parent == parent)
				return false;
			//Can't set a descendant of this as a parent.
			if(HasDescendant(parent))
				return false;
			var previous = (IHierarchy)this.parent;
			this.parent = parent;
			//Message<IParentMessage>(new ParentMessage(this, next, previous));

			//int sleeping = 0;
			//Extra previous and next checks against parent
			//in case an event changes the parent mid dispatch.
			if(previous != null && this.parent != previous)
			{
				previous.RemoveChild(this);
				//if(!IsFreeSleeping && previous.IsSleeping)
				//--sleeping;
			}
			if(parent != null && this.parent == parent)
			{
				index = Math.Max(0, Math.Min(index, parent.Children.Count));
				parent.AddChild(this, index);
				//if(!IsFreeSleeping && next.IsSleeping)
				//++sleeping;
			}
			//If parent becomes null, this won't get sent to anyone below...
			//...Which might really be intended/expected behavior.
			//Might still need to listen for parent changes in AtlasEngine.
			//Message<IParentMessage>(new ParentMessage(next, previous));

			SetParentIndex(index);
			//Sleeping += sleeping;
			Root = (IHierarchy)parent?.Root;
			//if(AutoDestroy && parent == null)
			//Dispose();
			return true;
		}

		public int ParentIndex
		{
			get { return parentIndex; }
			set { Parent?.SetChildIndex(this, value); }
		}

		public bool HasDescendant(IReadOnlyHierarchy descendant)
		{
			if(descendant == this)
				return false;
			while(descendant != null && descendant != this)
				descendant = descendant.Parent;
			return descendant == this;
		}

		public bool HasAncestor(IReadOnlyHierarchy ancestor)
		{
			return ancestor?.HasDescendant(this) ?? false;
		}

		public bool HasSibling(IReadOnlyHierarchy sibling)
		{
			if(sibling == this)
				return false;
			return Parent?.HasChild(sibling) ?? false;
		}

		public IReadOnlyHierarchy GetChild(int index)
		{
			return children[index];
		}

		public int GetChildIndex(IReadOnlyHierarchy child)
		{
			return children.IndexOf(child);
		}

		public bool SetChildIndex(IHierarchy child, int index)
		{
			int previous = children.IndexOf(child);

			if(previous == index)
				return true;
			if(previous < 0)
				return false;

			index = Math.Max(0, Math.Min(index, children.Count - 1));

			children.RemoveAt(previous);
			children.Insert(index, child);
			//Message<IChildrenMessage>(new ChildrenMessage(this));
			return true;
		}

		public bool SwapChildren(IHierarchy child1, IHierarchy child2)
		{
			if(child1 == null)
				return false;
			if(child2 == null)
				return false;
			int index1 = children.IndexOf(child1);
			int index2 = children.IndexOf(child2);
			return SwapChildren(index1, index2);
		}

		public bool SwapChildren(int index1, int index2)
		{
			if(!children.Swap(index1, index2))
				return false;
			//Message<IChildrenMessage>(new ChildrenMessage(this));
			return true;
		}

		IReadOnlyEngineList<IReadOnlyHierarchy> IReadOnlyHierarchy.Children
		{
			get { return children; }
		}

		public IReadOnlyEngineList<IHierarchy> Children
		{
			get { return (IReadOnlyEngineList<IHierarchy>)children; }
		}

		private void SetParentIndex(int value)
		{
			if(parentIndex == value)
				return;
			int previous = parentIndex;
			parentIndex = value;
			//Message<IParentIndexMessage>(new ParentIndexMessage(this, value, previous));
		}
	}

	public class Hierarchy<T> : Hierarchy, IHierarchy<T>
		where T : IHierarchy<T>
	{
		public new T Parent
		{
			get { return (T)base.Parent; }
			set { base.Parent = value; }
		}

		public bool SetParent(T parent, int index)
		{
			return base.SetParent(parent, index);
		}

		public bool SetChildIndex(T child, int index)
		{
			return base.SetChildIndex(child, index);
		}

		public bool SwapChildren(T child1, T child2)
		{
			return base.SwapChildren(child1, child2);
		}

		public new T GetChild(int index)
		{
			return (T)base.GetChild(index);
		}

		public T AddChild(T child)
		{
			return (T)base.AddChild(child);
		}

		public T AddChild(T child, int index)
		{
			return (T)base.AddChild(child, index);
		}

		public bool AddChildren(params T[] children)
		{
			return AddChildren(children as IHierarchy[]);
		}

		public bool AddChildren(int index, params T[] children)
		{
			return AddChildren(index, children as IHierarchy[]);
		}

		public T RemoveChild(T child)
		{
			return (T)base.RemoveChild(child);
		}

		public new T RemoveChild(int index)
		{
			return (T)base.RemoveChild(index);
		}

		public new T Root
		{
			get { return (T)base.Root; }
			protected set { base.Root = value; }
		}

		public new IReadOnlyEngineList<T> Children
		{
			get { return (IReadOnlyEngineList<T>)base.Children; }
		}

		public int GetChildIndex(T child)
		{
			throw new NotImplementedException();
		}

		public bool HasDescendant(T descendant)
		{
			return base.HasDescendant(descendant);
		}

		public bool HasAncestor(T ancestor)
		{
			return base.HasAncestor(ancestor);
		}

		public bool HasChild(T child)
		{
			return base.HasChild(child);
		}

		public bool HasSibling(T sibling)
		{
			return base.HasSibling(sibling);
		}

		public new bool SetParent(IHierarchy parent, int index)
		{
			return base.SetParent(parent, index);
		}

		public new bool SetChildIndex(IHierarchy child, int index)
		{
			return base.SetChildIndex(child, index);
		}

		public new bool SwapChildren(IHierarchy child1, IHierarchy child2)
		{
			return base.SwapChildren(child1, child2);
		}
	}
}
