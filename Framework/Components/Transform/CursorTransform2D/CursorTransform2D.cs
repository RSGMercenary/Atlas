using Microsoft.Xna.Framework;
using System;

namespace Atlas.Framework.Components.Transform
{
	public class CursorTransform2D : Transform2D, ICursorTransform2D
	{
		private bool followPosition = true;
		private bool followRotation = true;

		public CursorTransform2D()
		{

		}

		public bool FollowPosition
		{
			get { return followPosition; }
			set
			{
				if(followPosition == value)
					return;
				followPosition = value;
				//Send message
				Dirty();
			}
		}

		public bool FollowRotation
		{
			get { return followRotation; }
			set
			{
				if(followRotation == value)
					return;
				followRotation = value;
				//Send message
				Dirty();
			}
		}

		//This makes it so that the cursor is always following the
		//mouse (x,y) and is never rotating when it's in another parent
		//other than a HUD layer.
		protected override Matrix CreateLocalMatrix()
		{
			var position = Position;
			var rotation = Rotation;

			if(followPosition || followRotation)
			{
				var world = Matrix.Invert(Parent.Global);
				world.Decompose(out var scl, out var rot, out var pos);
				if(followPosition)
					position = Vector2.Transform(position, world);
				if(followRotation)
					rotation += (float)Math.Atan2(rot.Z, rot.W) * 2;
			}

			return Matrix.CreateScale(new Vector3(Scale, 1)) *
				   Matrix.CreateRotationZ(rotation) *
				   Matrix.CreateTranslation(new Vector3(position, 0));
		}
	}
}
