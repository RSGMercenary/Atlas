using Atlas.Framework.Geometry;
using System.Collections.Generic;

namespace Atlas.Framework.Components
{
	class Transform
	{
		private Vector3DC<Transform> position;
		private Vector3DC<Transform> rotation;
		private Vector3DC<Transform> scale;

		private Transform parent;
		private List<Transform> children = new List<Transform>();

		public Transform()
		{
			position = new Vector3DC<Transform>(this);
			rotation = new Vector3DC<Transform>(this);
			scale = new Vector3DC<Transform>(this);
		}

		public Vector3DC<Transform> Position
		{
			get
			{
				return position;
			}
		}

		public Vector3DC<Transform> Rotation
		{
			get
			{
				return rotation;
			}
		}

		public Vector3DC<Transform> Scale
		{
			get
			{
				return scale;
			}
		}

		public Transform Parent
		{
			get
			{
				return parent;
			}
			set
			{
				if(parent != value)
				{
					Transform previous = parent;
					parent = value;
					if(previous != null)
					{
						previous.RemoveChild(this);
					}
					if(parent != null)
					{
						parent.AddChild(this);
					}
				}
			}
		}

		public void SetParent(Transform parent, int index)
		{
			if(this.parent != parent)
			{
				Transform previousParent = this.parent;
				int previousIndex = -1;
				if(previousParent != null)
				{
					previousIndex = previousParent.children.IndexOf(this);
					previousParent.children.RemoveAt(previousIndex);
				}
				this.parent = parent;
				if(this.parent != null)
				{
					this.parent.children.Insert(index, this);
				}


			}
		}

		public Transform AddChild(Transform child)
		{
			return AddChild(child, children.Count);
		}

		public Transform AddChild(Transform child, int index)
		{
			if(child.parent != this)
			{
				if(child.parent != null)
				{

				}
				child.parent = this;
				children.Insert(index, child);
			}

			return null;
		}

		public Transform RemoveChild(Transform child)
		{
			if(child.parent == this)
			{
				child.parent = null;
				children.Remove(child);
			}
			return null;
		}
	}
}
