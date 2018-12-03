using Atlas.Core.Utilites;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.ECS.Entities.Messages;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public class Transform2D : AtlasComponent, ITransform2D
	{
		private bool recalculate = true;

		private Vector2 position;
		private Vector2 scale;
		private float rotation;

		public Transform2D() : this(new Vector2(0, 0)) { }
		public Transform2D(Vector2 position) : this(position, 0) { }
		public Transform2D(Vector2 position, float rotation) : this(position, rotation, new Vector2(1, 1)) { }

		public Transform2D(Vector2 position, float rotation, Vector2 scale)
		{
			Set(position, rotation, scale);
		}

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			entity.AddListener<IGlobalMatrixMessage>(SetMatrix);
			SetMatrix();
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			entity.RemoveListener<IGlobalMatrixMessage>(SetMatrix);
			base.RemovingManager(entity, index);
		}

		#region Position

		public Vector2 Position
		{
			get { return position; }
			set
			{
				if(position == value)
					return;
				position = value;
				//Send event
				SetMatrix();
			}
		}

		public float PositionX
		{
			get { return position.X; }
			set { Position = new Vector2(value, position.Y); }
		}

		public float PositionY
		{
			get { return position.Y; }
			set { Position = new Vector2(position.X, value); }
		}

		#endregion

		#region Scale

		public Vector2 Scale
		{
			get { return scale; }
			set
			{
				if(scale == value)
					return;
				scale = value;
				//Send event
				SetMatrix();
			}
		}

		public float ScaleX
		{
			get { return scale.X; }
			set { Scale = new Vector2(value, scale.Y); }
		}

		public float ScaleY
		{
			get { return scale.Y; }
			set { Scale = new Vector2(scale.X, value); }
		}

		public float ScaleXY
		{
			get { return (scale.X * scale.Y) / 2; }
			set { Scale = new Vector2(value, value); }
		}

		#endregion

		#region Rotation

		public float Rotation
		{
			get { return rotation; }
			set
			{
				value = (float)Clamp.Radians(value);
				if(rotation == value)
					return;
				rotation = value;
				//Send event
				SetMatrix();
			}
		}

		#endregion

		/// <summary>
		/// Set all Transform values in bulk before applying changes to the Entity's LocalMatrix.
		/// Reduces the amount of recalculations and messages that get sent.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <param name="scale"></param>
		public void Set(Vector2 position, float rotation, Vector2 scale)
		{
			Recalculate = false;
			Position = position;
			Rotation = rotation;
			Scale = scale;
			Recalculate = true;
		}

		protected bool Recalculate
		{
			get { return recalculate; }
			set
			{
				if(recalculate == value)
					return;
				recalculate = value;
				if(recalculate)
					SetMatrix();
			}
		}

		private void SetMatrix(IGlobalMatrixMessage message)
		{
			if(message.Hierarchy)
				SetMatrix();
		}

		protected void SetMatrix()
		{
			if(!recalculate)
				return;
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
