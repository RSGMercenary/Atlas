using Atlas.ECS.Components;

namespace Atlas.Framework.Components.Transform
{
	public interface ICursor2D : IComponent
	{
		/// <summary>
		/// When true, the cursor Entity will follow mouse position, regardless
		/// of where the cursor is in the Entity hierarchy.
		/// </summary>
		bool FollowPosition { get; set; }

		/// <summary>
		/// When true, the cursor Entity will follow a 
		/// </summary>
		bool FollowRotation { get; set; }

		bool Follow { set; }
	}
}
