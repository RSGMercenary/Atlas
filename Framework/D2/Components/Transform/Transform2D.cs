using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;
using System;

namespace Atlas.Framework.Components.Transform
{
	class Transform2D:AtlasComponent, ITransform2D
	{
		public ISignal<ITransform2D, ITransform2D, int> ChildAdded
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ISignal<ITransform2D, int, int, bool> ChildIndicesChanged
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ISignal<ITransform2D, ITransform2D, int> ChildRemoved
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IReadOnlyLinkList<ITransform2D> Children
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ITransform2D Parent
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public ISignal<ITransform2D, ITransform2D, ITransform2D> AncestorChanged
		{
			get
			{
				throw new NotImplementedException();
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