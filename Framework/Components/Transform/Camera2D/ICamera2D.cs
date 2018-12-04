using Atlas.ECS.Components;

namespace Atlas.Framework.Components.Transform
{
	public interface ICamera2D : IComponent
	{
		ITransform2D FollowPosition { get; set; }
		ITransform2D FollowRotation { get; set; }

		/// <summary>
		/// Sets the Camera's follow position and rotation to the same Entity.
		/// Used to put the Camera "on rails" and rigidly follow a target.
		/// </summary>
		ITransform2D Follow { set; }
	}
}
