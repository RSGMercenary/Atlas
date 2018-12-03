using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Components.Transform
{
	public interface ICamera2D : IComponent
	{
		IEntity FollowPosition { get; set; }
		IEntity FollowRotation { get; set; }

		/// <summary>
		/// Sets the Camera's follow position and rotation to the same Entity.
		/// Used to put the Camera "on rails" and rigidly follow a target.
		/// </summary>
		IEntity Follow { set; }
	}
}
