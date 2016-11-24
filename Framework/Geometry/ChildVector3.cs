namespace Atlas.Framework.Geometry
{
	class ChildVector3<T>:Vector3, IChildVector3<T>
	{
		private readonly T parent;

		public ChildVector3(T parent, float x = 0, float y = 0, float z = 0) : base(x, y, z)
		{
			this.parent = parent;
		}

		public T Parent
		{
			get
			{
				return parent;
			}
		}
	}
}
