using Atlas.ECS.Components;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Physics
{
	public class Velocity2D : AtlasComponent, IVelocity2D
	{
		private Vector2 vector = new Vector2(0, 0);
		private float rotation = 0;

		public Vector2 Vector
		{
			get { return vector; }
			set
			{
				if(vector == value)
					return;
				vector = value;
			}
		}

		public float Rotation
		{
			get { return rotation; }
			set
			{
				if(rotation == value)
					return;
				rotation = value;
			}
		}
	}
}