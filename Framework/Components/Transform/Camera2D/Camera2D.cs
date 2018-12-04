using Atlas.ECS.Components;

namespace Atlas.Framework.Components.Transform
{
	public class Camera2D : AtlasComponent, ICamera2D
	{
		public ITransform2D FollowPosition { get; set; }
		public ITransform2D FollowRotation { get; set; }

		public Camera2D() { }

		public Camera2D(ITransform2D position) : this(position, null) { }

		public Camera2D(ITransform2D position, ITransform2D rotation)
		{
			FollowPosition = position;
			FollowRotation = rotation;
		}

		public ITransform2D Follow
		{
			set
			{
				FollowPosition = value;
				FollowRotation = value;
			}
		}
	}
}