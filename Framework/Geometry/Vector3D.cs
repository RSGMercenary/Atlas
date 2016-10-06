namespace Atlas.Framework.Geometry
{
	class Vector3D
	{
		private float x = 0;
		private float y = 0;
		private float z = 0;

		public Vector3D()
		{

		}

		public float X
		{
			get
			{
				return x;
			}
			set
			{
				if(x != value)
				{
					float previous = x;
					x = value;
				}
			}
		}

		public float Y
		{
			get
			{
				return y;
			}
			set
			{
				if(y != value)
				{
					float previous = y;
					y = value;
				}
			}
		}

		public float Z
		{
			get
			{
				return z;
			}
			set
			{
				if(z != value)
				{
					float previous = z;
					z = value;
				}
			}
		}
	}
}
