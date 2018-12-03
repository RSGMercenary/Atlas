using Microsoft.Xna.Framework;
using System;

namespace Atlas.Framework.Components.Transform
{
	public class CursorTransform2D : Transform2D, ICursorTransform2D
	{
		private int disabled = 0;

		public CursorTransform2D()
		{

		}

		public bool IsDisabled
		{
			get { return disabled > 0; }
			set
			{
				if(value)
					++Disabled;
				else
					--Disabled;
			}
		}

		public int Disabled
		{
			get { return disabled; }
			private set
			{
				if(disabled == value)
					return;
				int previous = disabled;
				disabled = value;
				//Send message
				if(value > 0 && previous <= 0)
				{
					SetMatrix();
				}
				else if(value <= 0 && previous > 0)
				{
					SetMatrix();
				}
			}
		}

		//This makes it so that the cursor is always following the
		//mouse (x,y) and is never rotating when it's in another parent
		//other than a HUD layer.
		protected override Matrix CreateLocalMatrix()
		{
			if(IsDisabled)
				return base.CreateLocalMatrix();

			var world = Matrix.Invert(Manager.Parent.GlobalMatrix);
			world.Decompose(out var scl, out var rot, out var pos);
			var position = Vector2.Transform(Position, world);
			var rotation = (float)Math.Atan2(rot.Z, rot.W) * 2; ;
			return Matrix.CreateScale(new Vector3(Scale, 1)) *
				   Matrix.CreateRotationZ(Rotation + rotation) *
				   Matrix.CreateTranslation(new Vector3(position, 0));
		}
	}
}
