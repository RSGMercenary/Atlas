using Atlas.Core.Messages;
using Atlas.Core.Utilites;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.ECS.Entities.Messages;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public class Transform2D : AtlasComponent, ITransform2D
	{
		#region Fields

		private ITransform2D parent;
		private Matrix matrix;
		private Matrix local;

		private Vector2 position;
		private Vector2 scale;
		private float rotation;

		private bool dirty = false;
		private bool recalculate = true;

		#endregion

		#region Constructors

		public Transform2D() : this(new Vector2(0, 0)) { }
		public Transform2D(Vector2 position) : this(position, 0) { }
		public Transform2D(Vector2 position, float rotation) : this(position, rotation, new Vector2(1, 1)) { }

		public Transform2D(Vector2 position, float rotation, Vector2 scale)
		{
			Set(position, rotation, scale);
		}

		#endregion

		#region Add / Remove Manager

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			entity.AddListener<IParentMessage>(ParentChanged);
			entity.AddListener<IComponentAddMessage>(ComponentAdded);
			entity.AddListener<IComponentRemoveMessage>(ComponentRemoved);
			Parent = entity.GetAncestorComponent<ITransform2D>();
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			entity.RemoveListener<IParentMessage>(ParentChanged);
			entity.RemoveListener<IComponentAddMessage>(ComponentAdded);
			entity.RemoveListener<IComponentRemoveMessage>(ComponentRemoved);
			Parent = null;
			base.RemovingManager(entity, index);
		}

		#endregion

		#region Messages

		private void ParentChanged(IParentMessage message)
		{
			SetParent(message);
		}

		private void ComponentAdded(IComponentAddMessage message)
		{
			if(message.Key != typeof(ITransform2D))
				return;
			SetParent(message);
		}

		private void ComponentRemoved(IComponentRemoveMessage message)
		{
			if(message.Key != typeof(ITransform2D))
				return;
			SetParent(message);
		}

		private void SetParent(IMessage<IEntity> message)
		{
			if(Manager.HasAncestor(message.Messenger))
				Parent = Manager.GetAncestorComponent<ITransform2D>();
		}

		#endregion

		protected void Dirty()
		{
			dirty = true;
		}

		public ITransform2D Parent
		{
			get { return parent; }
			private set
			{
				if(parent == value)
					return;
				var previous = parent;
				parent = value;
			}
		}

		public Matrix Matrix
		{
			get
			{
				SetMatrix();
				return matrix;
			}
		}

		protected void SetMatrix()
		{
			if(dirty)
				local = CreateLocalMatrix();
			dirty = false;
			matrix = local;
			if(parent != null)
				matrix *= parent.Matrix;
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
				Dirty();
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
				Dirty();
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
				Dirty();
			}
		}

		#endregion

		public void Set(Vector2 position, float rotation, Vector2 scale)
		{
			Position = position;
			Rotation = rotation;
			Scale = scale;
		}

		protected virtual Matrix CreateLocalMatrix()
		{
			return Matrix.CreateScale(new Vector3(scale, 1)) *
				   Matrix.CreateRotationZ(rotation) *
				   Matrix.CreateTranslation(new Vector3(position, 0));
		}
	}
}
