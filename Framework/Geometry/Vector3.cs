namespace Atlas.Framework.Geometry
{
	class Vector3:Vector2, IVector3
	{
		private float z = 0;

		public Vector3(float x = 0, float y = 0, float z = 0) : base(x, y)
		{
			Z = z;
		}

		override public string ToString()
		{
			return "(" + X + ", " + Y + ", " + Z + ")";
		}

		public float Z
		{
			get
			{
				return z;
			}
			set
			{
				if(float.IsNaN(value))
					return;
				if(z != value)
				{
					float previous = z;
					z = value;
				}
			}
		}
	}
}
