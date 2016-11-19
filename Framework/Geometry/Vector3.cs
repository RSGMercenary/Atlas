namespace Atlas.Framework.Geometry
{
	class Vector3
	{
		private float x = 0;
		private float y = 0;
		private float z = 0;

		public float X
		{
			get
			{
				return x;
			}
			set
			{
				if(float.IsNaN(value))
					return;
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
				if(float.IsNaN(value))
					return;
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
