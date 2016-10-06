namespace Atlas.Framework.Geometry
{
	class Vector3DC<T>:Vector3D
	{
		private T parent;

		public Vector3DC(T parent)
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
