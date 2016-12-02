namespace Atlas.Framework.Components.Transform
{/*
	class Transform2D:AtlasComponent<ITransform2D>, ITransform2D
	{
		private float positionX = 0;
		private float positionY = 0;
		private float positionZ = 0;

		private float rotationX = 0;
		private float rotationY = 0;
		private float rotationZ = 0;

		private float scaleX = 0;
		private float scaleY = 0;
		private float scaleZ = 0;


		private IChildSpace<ITransform2D> global;
		private IChildSpace<ITransform2D> local;

		private ITransform2D parent;
		private LinkList<ITransform2D> children = new LinkList<ITransform2D>();
		bool overrideManagerHierarchy = true;

		private Signal<ITransform2D, ITransform2D, ITransform2D> parentChanged = new Signal<ITransform2D, ITransform2D, ITransform2D>();
		private Signal<ITransform2D, int, int> parentIndexChanged = new Signal<ITransform2D, int, int>();
		private Signal<ITransform2D, ITransform2D, int> childAdded = new Signal<ITransform2D, ITransform2D, int>();
		private Signal<ITransform2D, ITransform2D, int> childRemoved = new Signal<ITransform2D, ITransform2D, int>();
		private Signal<ITransform2D, int, int, bool> childIndicesChanged = new Signal<ITransform2D, int, int, bool>();

		public Transform2D()
		{
			global = new ChildSpace<ITransform2D>(this);
			local = new ChildSpace<ITransform2D>(this);
		}

		public ISignal<ITransform2D, ITransform2D, ITransform2D> ParentChanged { get { return parentChanged; } }
		public ISignal<ITransform2D, int, int> ParentIndexChanged { get { return parentIndexChanged; } }
		public ISignal<ITransform2D, ITransform2D, int> ChildAdded { get { return childAdded; } }
		public ISignal<ITransform2D, ITransform2D, int> ChildRemoved { get { return childRemoved; } }

		public ISignal<ITransform2D, int, int, bool> ChildIndicesChanged
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IChildSpace<ITransform2D> Global
		{
			get
			{
				return global;
			}
		}

		public IChildSpace<ITransform2D> Local
		{
			get
			{
				return local;
			}
		}

		public ITransform2D Root
		{
			get
			{
				if(Engine == null)
					return null;
				if(Engine.Manager == null)
					return null;
				return Engine.Manager.GetComponent<ITransform2D>();
			}
		}

		public ITransform2D Parent
		{
			get
			{
				return parent;
			}
			set
			{
				SetParent(value);
			}
		}

		public IReadOnlyLinkList<ITransform2D> Children
		{
			get
			{
				return children;
			}
		}

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			SetManagerHierarchy();
		}

		public bool OverrideManagerHierarchy
		{
			get
			{
				return overrideManagerHierarchy;
			}
			set
			{
				if(overrideManagerHierarchy != value)
				{
					bool previous = overrideManagerHierarchy;
					overrideManagerHierarchy = value;

					SetManagerHierarchy();
				}
			}
		}

		private void SetManagerHierarchy()
		{
			if(overrideManagerHierarchy)
			{
				if(Manager != null && parent != null && parent.Manager != null)
				{
					Manager.Parent = parent.Manager;
				}
			}
		}

		public bool SetParent(ITransform2D parent = null, int index = int.MaxValue)
		{
			//Parents are the same.
			if(this.parent == parent)
				return false;
			//Can't set a parent's ancestor (this) as a child.
			if(HasDescendant(parent))
				return false;
			ITransform2D previousParent = this.parent;
			int previousIndex = -1;
			this.parent = parent;
			if(previousParent != null)
			{
				previousParent.ManagerAdded.Remove(ComponentManagerAdded);
				previousIndex = previousParent.GetChildIndex(this);
				previousParent.RemoveChild(previousIndex);
			}
			if(parent != null)
			{
				parent.ManagerAdded.Add(ComponentManagerAdded);
				SetManagerHierarchy();
				index = Math.Max(0, Math.Min(index, parent.Children.Count));
				parent.AddChild(this, index);
			}
			else
			{
				index = -1;
			}
			parentChanged.Dispatch(this, parent, previousParent);
			if(index != previousIndex)
			{
				parentIndexChanged.Dispatch(this, index, previousIndex);
			}
			return true;
		}

		private void ComponentManagerAdded(IComponent component, IEntity entity, int index)
		{
			SetManagerHierarchy();
		}

		public bool HasChild(ITransform2D child)
		{
			return children.Contains(child);
		}

		public int GetChildIndex(ITransform2D child)
		{
			return children.GetIndex(child);
		}

		public bool SetChildIndex(ITransform2D child, int index)
		{
			int previous = children.GetIndex(child);

			if(previous == index)
				return true;
			if(previous < 0)
				return false;

			index = Math.Max(0, Math.Min(index, children.Count - 1));

			int next = index;

			children.Remove(previous);
			children.Add(child, next);

			if(next > previous)
			{
				childIndicesChanged.Dispatch(this, previous, next, true);

				//Children shift down 0<-[1]
				for(index = previous; index < next; ++index)
				{
					child = children[index];
					child.ParentIndexChanged.Dispatch(child, index, index + 1);
				}
			}
			else
			{
				childIndicesChanged.Dispatch(this, next, previous, true);

				//Children shift up [0]->1
				for(index = previous; index > next; --index)
				{
					child = children[index];
					child.ParentIndexChanged.Dispatch(child, index, index - 1);
				}
			}
			child.ParentIndexChanged.Dispatch(child, next, previous);
			return true;
		}

		public bool HasDescendant(ITransform2D relative)
		{
			while(relative != null && relative != this)
			{
				relative = relative.Parent;
			}
			return relative == this;
		}

		public ITransform2D GetChild(int index)
		{
			return children.Get(index);
		}

		public ITransform2D AddChild(ITransform2D child)
		{
			return AddChild(child, children.Count);
		}

		public ITransform2D AddChild(ITransform2D child, int index)
		{
			if(child == null)
				return null;
			if(child.Parent == this)
			{
				if(!children.Contains(child))
				{
					children.Add(child, index);
					childAdded.Dispatch(this, child, index);
					childIndicesChanged.Dispatch(this, index, children.Count, true);
					for(int i = index + 1; i < children.Count; ++i)
					{
						ITransform2D sibling = children[i];
						sibling.ParentIndexChanged.Dispatch(sibling, i, i - 1);
					}
				}
				else
				{
					SetChildIndex(child, index);
				}
			}
			else
			{
				child.SetParent(this, index);
			}
			return child;
		}

		public ITransform2D RemoveChild(ITransform2D child)
		{
			if(child == null)
				return null;
			if(child.Parent != this)
			{
				if(!children.Contains(child))
					return null;
				int index = children.GetIndex(child);
				children.Remove(index);
				childRemoved.Dispatch(this, child, index);
				childIndicesChanged.Dispatch(this, index, children.Count, true);
				for(int i = index; i < children.Count; ++i)
				{
					ITransform2D sibling = children[i];
					sibling.ParentIndexChanged.Dispatch(sibling, i, i + 1);
				}
			}
			else
			{
				child.SetParent(null, -1);
			}
			return child;
		}

		public ITransform2D RemoveChild(int index)
		{
			return RemoveChild(children[index]);
		}

		public bool RemoveChildren()
		{
			if(children.Count <= 0)
				return false;
			while(children.First != null)
			{
				children.Last.Value.Parent = null;
			}
			return true;
		}
	}*/
}