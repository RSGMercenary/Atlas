using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Components.Transform
{
	public class Camera2D : AtlasComponent, ICamera2D
	{
		public IEntity FollowPosition { get; set; }
		public IEntity FollowRotation { get; set; }

		public Camera2D() { }

		public Camera2D(IEntity position) : this(position, null) { }

		public Camera2D(IEntity position, IEntity rotation)
		{
			FollowPosition = position;
			FollowRotation = rotation;
		}

		public IEntity Follow
		{
			set
			{
				FollowPosition = value;
				FollowRotation = value;
			}
		}
	}
}