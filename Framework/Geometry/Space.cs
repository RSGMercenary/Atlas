namespace Atlas.Framework.Geometry
{
	class Space:ISpace
	{
		private ChildVector3<ISpace> position;
		private ChildVector3<ISpace> rotation;
		private ChildVector3<ISpace> scale;
		private ChildVector3<ISpace> skew;

		public Space()
		{
			position = new ChildVector3<ISpace>(this);
			rotation = new ChildVector3<ISpace>(this);
			scale = new ChildVector3<ISpace>(this);
			skew = new ChildVector3<ISpace>(this);
		}

		public IChildVector3<ISpace> Position
		{
			get
			{
				return position;
			}
		}

		public IChildVector3<ISpace> Rotation
		{
			get
			{
				return rotation;
			}
		}

		public IChildVector3<ISpace> Scale
		{
			get
			{
				return scale;
			}
		}

		public IChildVector3<ISpace> Skew
		{
			get
			{
				return skew;
			}
		}
	}
}
