using Atlas.Core.Utilites;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public class Transform2D : AtlasComponent, ITransform2D
	{
		private Vector2 position;
		private Vector2 scale;
		private float rotation;

		public Transform2D() : this(new Vector2(0, 0)) { }
		public Transform2D(Vector2 position) : this(position, 0) { }
		public Transform2D(Vector2 position, float rotation) : this(position, rotation, new Vector2(1, 1)) { }

		public Transform2D(Vector2 position, float rotation, Vector2 scale)
		{
			Position = position;
			Rotation = rotation;
			Scale = scale;
		}

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			SetMatrix();
		}

		public virtual Vector2 Position
		{
			get { return position; }
			set
			{
				if(position == value)
					return;
				position = value;
				SetMatrix();
			}
		}

		public virtual Vector2 Scale
		{
			get { return scale; }
			set
			{
				if(scale == value)
					return;
				scale = value;
				SetMatrix();
			}
		}

		public virtual float Rotation
		{
			get { return rotation; }
			set
			{
				value = (float)Clamp.Radians(value);
				if(rotation == value)
					return;
				rotation = value;
				SetMatrix();
			}
		}

		protected void SetMatrix()
		{
			if(Manager != null)
				Manager.LocalMatrix = CreateLocalMatrix();
		}

		protected virtual Matrix CreateLocalMatrix()
		{
			return Matrix.CreateScale(new Vector3(scale, 1)) *
				   Matrix.CreateRotationZ(rotation) *
				   Matrix.CreateTranslation(new Vector3(position, 0));
		}
	}
}
