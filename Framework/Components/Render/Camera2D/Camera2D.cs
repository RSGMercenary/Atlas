using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Components.Render
{
	public class Camera2D : AtlasComponent, ICamera2D
	{
		public IEntity Position { get; set; }
		public IEntity Rotation { get; set; }

		public Camera2D() { }

		public Camera2D(IEntity position) : this(position, null) { }

		public Camera2D(IEntity position, IEntity rotation)
		{
			Position = position;
			Rotation = rotation;
		}
	}
}