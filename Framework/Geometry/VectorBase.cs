namespace Atlas.Framework.Geometry
{
	abstract class VectorBase<TReturn> where TReturn : VectorBase<TReturn>
	{

	}

	abstract class Vector2Base<TReturn>:VectorBase<TReturn> where TReturn : VectorBase<TReturn>
	{
		private float x = 0;
		private float y = 0;
	}

	abstract class Vector3Base<TReturn>:Vector2Base<TReturn> where TReturn : VectorBase<TReturn>
	{
		private float z = 0;
	}
}
