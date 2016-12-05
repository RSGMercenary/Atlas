using System;

namespace Atlas.Framework.Geometry
{
	class Vector3:Vector3<Vector3>
	{
		public static readonly IReadOnlyVector3 Up = new Vector3(0, 1, 0);
		public static readonly IReadOnlyVector3 Down = new Vector3(0, -1, 0);
		public static readonly IReadOnlyVector3 Left = new Vector3(-1, 0, 0);
		public static readonly IReadOnlyVector3 Right = new Vector3(0, 1, 0);
		public static readonly IReadOnlyVector3 Forward = new Vector3(0, 0, 1);
		public static readonly IReadOnlyVector3 Backward = new Vector3(0, 0, -1);

		public Vector3()
		{

		}

		public Vector3(float x = 0, float y = 0, float z = 0) : base(x, y, z)
		{

		}

		public Vector3(IReadOnlyVector3 vector) : base(vector)
		{

		}
	}

	class Vector3<TReturn>:Vector2<TReturn>, IVector3<TReturn> where TReturn : Vector3<TReturn>
	{
		private float z = 0;

		public Vector3()
		{

		}

		public Vector3(float x = 0, float y = 0, float z = 0)
		{
			Set3(x, y, z);
		}

		public Vector3(IReadOnlyVector3 vector)
		{
			Set3(vector);
		}

		override public string ToString()
		{
			return "(" + X + ", " + Y + ", " + Z + ")";
		}

		#region Vector3 - Properties

		public virtual float Z
		{
			get { return z; }
			set { z = value; }
		}

		public float LengthSquared3
		{
			get { return LengthSquared2 + Z * Z; }
		}

		public float Length3
		{
			get { return (float)Math.Sqrt(LengthSquared3); }
		}

		#endregion

		#region Vector - Floats

		public override TReturn Set(float value)
		{
			return Set3(value, value, value);
		}

		public override TReturn Add(float value)
		{
			return Add3(value, value, value);
		}

		public override TReturn Subtract(float value)
		{
			return Subtract3(value, value, value);
		}

		public override TReturn Multiply(float value)
		{
			return Multiply3(value, value, value);
		}

		public override TReturn Divide(float value)
		{
			return Divide3(value, value, value);
		}

		#endregion

		#region Vector3 - Floats

		public TReturn Set3(float x, float y, float z)
		{
			Set2(x, y);
			Z = z;
			return this as TReturn;
		}

		public TReturn Add3(float x = 0, float y = 0, float z = 0)
		{
			Add2(x, y);
			Z += z;
			return this as TReturn;
		}

		public TReturn Subtract3(float x = 0, float y = 0, float z = 0)
		{
			Subtract2(x, y);
			Z -= z;
			return this as TReturn;
		}

		public TReturn Multiply3(float x = 1, float y = 1, float z = 1)
		{
			Multiply2(x, y);
			Z *= z;
			return this as TReturn;
		}

		public TReturn Divide3(float x = 1, float y = 1, float z = 1)
		{
			Divide2(x, y);
			Z /= z;
			return this as TReturn;
		}

		public TReturn Reflect3(float x, float y, float z)
		{
			//Is this right?
			float dot = 2 * Dot3(x, y, z);
			X -= dot * x;
			Y -= dot * y;
			Z -= dot * z;
			return this as TReturn;
		}

		public float Dot3(float x, float y, float z)
		{
			return X * x + Y * y + Z * z;
		}

		public float Cross3(float x, float y, float z)
		{
			return X * x - Y * y - Z * z;
		}

		public float DistanceSquared3(float x, float y, float z)
		{
			float deltaX = X - x;
			float deltaY = Y - y;
			float deltaZ = Z - z;
			return deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
		}

		public float Distance3(float x, float y, float z)
		{
			return (float)Math.Sqrt(DistanceSquared3(x, y, z));
		}

		#endregion

		#region Vector3 - Vectors

		public TReturn Set3(IReadOnlyVector3 vector)
		{
			return Set3(vector.X, vector.Y, vector.Z);
		}

		public TReturn Add3(IReadOnlyVector3 vector)
		{
			return Add3(vector.X, vector.Y, vector.Z);
		}

		public TReturn Subtract3(IReadOnlyVector3 vector)
		{
			return Subtract3(vector.X, vector.Y, vector.Z);
		}

		public TReturn Multiply3(IReadOnlyVector3 vector)
		{
			return Multiply3(vector.X, vector.Y, vector.Z);
		}

		public TReturn Divide3(IReadOnlyVector3 vector)
		{
			return Divide3(vector.X, vector.Y, vector.Z);
		}

		public float Dot3(IReadOnlyVector3 vector)
		{
			return Dot3(vector.X, vector.Y, vector.Z);
		}

		public float Cross3(IReadOnlyVector3 vector)
		{
			return Cross3(vector.X, vector.Y, vector.Z);
		}

		public float DistanceSquared3(IReadOnlyVector3 vector)
		{
			return DistanceSquared3(vector.X, vector.Y, vector.Z);
		}

		public float Distance3(IReadOnlyVector3 vector)
		{
			return Distance3(vector.X, vector.Y, vector.Z);
		}

		public TReturn Normalize3(float length = 1)
		{
			float ratio = Math.Abs(length) / Length3;
			return Multiply3(ratio, ratio, ratio);
		}

		public TReturn Reflect3(IReadOnlyVector3 vector)
		{
			Vector3 normal = new Vector3(vector).Normalize3();
			return Reflect3(normal.X, normal.Y, normal.Z);
		}

		#endregion
	}
}