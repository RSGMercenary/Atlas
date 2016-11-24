namespace Atlas.Framework.Geometry
{
	class ChildSpace<T>:Space, IChildSpace<T>
	{
		private readonly T parent;

		public ChildSpace(T parent)
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
