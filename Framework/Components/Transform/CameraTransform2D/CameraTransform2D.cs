using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public class CameraTransform2D : Transform2D, ICameraTransform2D
	{
		private Vector2 center;

		public CameraTransform2D()
		{

		}

		public Vector2 Center
		{
			get { return center; }
			set
			{
				if(center == value)
					return;
				center = value;
				SetMatrix();
			}
		}

		public void Set(Vector2 position, float rotation, Vector2 scale, Vector2 center)
		{
			Recalculate = false;
			Position = position;
			Rotation = rotation;
			Scale = scale;
			Center = center;
			Recalculate = true;
		}

		//This makes it so the camera always has the target centered on screen
		//regardless of where it is in the hierarchy.
		protected override Matrix CreateLocalMatrix()
		{
			return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
				   Matrix.CreateRotationZ(-Rotation) *
				   Matrix.CreateScale(new Vector3(Scale, 1)) *
				   Matrix.CreateTranslation(new Vector3(Center, 0));
		}
	}
}
