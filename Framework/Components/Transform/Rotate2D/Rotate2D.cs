using Atlas.ECS.Components;

namespace Atlas.Framework.Components.Transform
{
	public class Rotate2D : AtlasComponent, IRotate2D
	{
		private float torque = 0;

		public Rotate2D() { }

		public Rotate2D(float torque)
		{
			Torque = torque;
		}

		public float Torque
		{
			get { return torque; }
			set
			{
				if(torque == value)
					return;
				torque = value;
			}
		}
	}
}
