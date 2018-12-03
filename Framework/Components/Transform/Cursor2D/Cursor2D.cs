using Atlas.ECS.Components;

namespace Atlas.Framework.Components.Transform
{
	public class Cursor2D : AtlasComponent, ICursor2D
	{
		private bool followPosition = true;
		private bool followRotation = true;

		public bool FollowPosition
		{
			get { return followPosition; }
			set
			{
				if(followPosition == value)
					return;
				followPosition = value;
				//Send message
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
			}
		}

		public bool Follow
		{
			set
			{
				FollowPosition = value;
				FollowRotation = value;
			}
		}
	}
}
