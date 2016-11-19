namespace Atlas.Framework.Geometry
{
	class FVector3<T>:Vector3
	{
		readonly T parent;

		public FVector3(T parent)
		{
			this.parent = parent;
		}
	}
}
