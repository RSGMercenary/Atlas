using Atlas.Components;
using Atlas.LinkList;

namespace Atlas.Framework.Components
{
	class Transform:AtlasComponent, ITransform
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

		private Transform parent;
		private LinkList<Transform> children = new LinkList<Transform>();

		public Transform()
		{

		}

		public float PositionX
		{
			get
			{
				return positionX;
			}
			set
			{
				if(float.IsNaN(value))
					return;
				if(positionX == value)
					return;
				float previous = positionX;
				positionX = value;

				ILinkListNode<Transform> current = children.First;
				while(current != null)
				{

					current = current.Next;
				}
			}
		}

		public float PositionY
		{
			get
			{
				return positionY;
			}
		}

		public float PositionZ
		{
			get
			{
				return positionZ;
			}
			set
			{
				if(float.IsNaN(value))
					return;
			}
		}
	}
}
