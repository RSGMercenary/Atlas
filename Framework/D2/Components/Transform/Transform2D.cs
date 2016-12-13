using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;
using System;

namespace Atlas.Framework.D2.Components.Transform
{
	class Transform2D:AtlasComponent, ITransform2D
	{
		private LinkList<ITransform2D> children = new LinkList<ITransform2D>();
		private ITransform2D parent;
		private int parentIndex = -1;

		private Signal<ITransform2D, ITransform2D, ITransform2D, ITransform2D> ancestorChanged = new Signal<ITransform2D, ITransform2D, ITransform2D, ITransform2D>();
		private Signal<ITransform2D, ITransform2D, int> childAdded = new Signal<ITransform2D, ITransform2D, int>();
		private Signal<ITransform2D, ITransform2D, int> childRemoved = new Signal<ITransform2D, ITransform2D, int>();
		private Signal<ITransform2D, int, int, bool> childIndicesChanged = new Signal<ITransform2D, int, int, bool>();

		public Transform2D()
		{

		}

		public ISignal<ITransform2D, ITransform2D, int> ChildAdded { get { return childAdded; } }
		public ISignal<ITransform2D, ITransform2D, int> ChildRemoved { get { return childRemoved; } }
		public ISignal<ITransform2D, int, int, bool> ChildIndicesChanged { get { return childIndicesChanged; } }

		public IReadOnlyLinkList<ITransform2D> Children
		{
			get
			{
				return children;
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

		public ISignal<ITransform2D, ITransform2D, ITransform2D, ITransform2D> AncestorChanged
		{
			get
			{
				return ancestorChanged;
			}
		}

		public ISignal<ITransform2D, int, int> ParentIndexChanged
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ITransform2DManager Root
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ISignal<ITransform2D, ITransform2DManager, ITransform2DManager, ITransform2D> RootChanged
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ISignal<ITransform2D, ITransform2D, ITransform2D, ITransform2D> IHierarchy<ITransform2D, ITransform2DManager>.AncestorChanged
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ITransform2D AddChild(ITransform2D child)
		{
			throw new NotImplementedException();
		}

		public ITransform2D AddChild(ITransform2D child, int index)
		{
			throw new NotImplementedException();
		}

		public ITransform2D GetChild(int index)
		{
			throw new NotImplementedException();
		}

		public int GetChildIndex(ITransform2D child)
		{
			throw new NotImplementedException();
		}

		public bool HasAncestor(ITransform2D ancestor)
		{
			throw new NotImplementedException();
		}

		public bool HasChild(ITransform2D child)
		{
			throw new NotImplementedException();
		}

		public bool HasDescendant(ITransform2D descendant)
		{
			throw new NotImplementedException();
		}

		public ITransform2D RemoveChild(int index)
		{
			throw new NotImplementedException();
		}

		public ITransform2D RemoveChild(ITransform2D child)
		{
			throw new NotImplementedException();
		}

		public bool RemoveChildren()
		{
			throw new NotImplementedException();
		}

		public bool SetChildIndex(ITransform2D child, int index)
		{
			throw new NotImplementedException();
		}

		public bool SetParent(ITransform2D parent = null, int index = int.MaxValue)
		{
			throw new NotImplementedException();
		}
	}
}