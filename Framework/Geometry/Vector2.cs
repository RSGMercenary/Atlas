namespace Atlas.Framework.Geometry
{
	class Vector2:IVector2
	{
		private float x = 0;
		private float y = 0;

		public Vector2(float x = 0, float y = 0)
		{
			X = x;
			Y = y;
		}

		override public string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}

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
	}
}
