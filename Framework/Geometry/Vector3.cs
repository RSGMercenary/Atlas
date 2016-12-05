using System;

namespace Atlas.Framework.Geometry
{
	class Vector3:IVector3<Vector3>
	{
		public static readonly IReadOnlyVector3 Up = new Vector3(0, 1, 0);
		public static readonly IReadOnlyVector3 Down = new Vector3(0, -1, 0);
		public static readonly IReadOnlyVector3 Left = new Vector3(-1, 0, 0);
		public static readonly IReadOnlyVector3 Right = new Vector3(0, 1, 0);
		public static readonly IReadOnlyVector3 Forward = new Vector3(0, 0, 1);
		public static readonly IReadOnlyVector3 Backward = new Vector3(0, 0, -1);

		private float x = 0;
		private float y = 0;
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

		#region Properties

		public virtual float X
		{
			get { return x; }
			set { x = value; }
		}

		public virtual float Y
		{
			get { return y; }
			set { y = value; }
		}

		public virtual float Z
		{
			get { return z; }
			set { z = value; }
		}

		public float LengthSquared2
		{
			get { return X * X + Y * Y; }
		}

		public float LengthSquared3
		{
			get { return LengthSquared2 + Z * Z; }
		}

		public float Length2
		{
			get { return (float)Math.Sqrt(LengthSquared2); }
		}

		public float Length3
		{
			get { return (float)Math.Sqrt(LengthSquared3); }
		}

		public float RadiansZ
		{
			get { return (float)Math.Atan2(y, x); }
		}

		public float DegreesZ
		{
			get { return RadiansZ * (float)(180 / Math.PI); }
		}

		#endregion

		#region Vector Float Params

		public Vector3 Set(float value)
		{
			return Set3(value, value, value);
		}

		public Vector3 Add(float value)
		{
			return Add3(value, value, value);
		}

		public Vector3 Subtract(float value)
		{
			return Subtract3(value, value, value);
		}

		public Vector3 Multiply(float value)
		{
			return Multiply3(value, value, value);
		}

		public Vector3 Divide(float value)
		{
			return Divide3(value, value, value);
		}

		#endregion

		#region Vector2 Float Params

		public Vector3 Set2(float x, float y)
		{
			X = x;
			Y = y;
			return this;
		}

		public Vector3 Add2(float x = 0, float y = 0)
		{
			X += x;
			Y += y;
			return this;
		}

		public Vector3 Subtract2(float x = 0, float y = 0)
		{
			X -= x;
			Y -= y;
			return this;
		}

		public Vector3 Multiply2(float x = 1, float y = 1)
		{
			X *= x;
			Y *= y;
			return this;
		}

		public Vector3 Divide2(float x = 1, float y = 1)
		{
			X /= x;
			Y /= y;
			return this;
		}

		public Vector3 Reflect2(float x, float y)
		{
			float dot = 2 * Dot2(x, y);
			X -= dot * x;
			Y -= dot * y;
			return this;
		}

		public float Dot2(float x, float y)
		{
			return X * x + Y * y;
		}

		public float Cross2(float x, float y)
		{
			return X * x - Y * y;
		}

		public Vector3 Normalize2(float length = 1)
		{
			float ratio = Math.Abs(length) / Length2;
			return Multiply2(ratio, ratio);
		}

		#endregion

		#region Vector2 - Vectors

		public Vector3 Set3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
			return this;
		}

		public Vector3 Add3(float x = 0, float y = 0, float z = 0)
		{
			X += x;
			Y += y;
			Z += z;
			return this;
		}

		public Vector3 Subtract3(float x = 0, float y = 0, float z = 0)
		{
			X -= x;
			Y -= y;
			Z -= z;
			return this;
		}

		public Vector3 Multiply3(float x = 1, float y = 1, float z = 1)
		{
			X *= x;
			Y *= y;
			Z *= z;
			return this;
		}

		public Vector3 Divide3(float x = 1, float y = 1, float z = 1)
		{
			X /= x;
			Y /= y;
			Z /= z;
			return this;
		}

		public Vector3 Reflect3(float x, float y, float z)
		{
			//Is this right?
			float dot = 2 * Dot3(x, y, z);
			X -= dot * x;
			Y -= dot * y;
			Z -= dot * z;
			return this;
		}

		public float Dot3(float x, float y, float z)
		{
			return X * x + Y * y + Z * z;
		}

		public float Cross3(float x, float y, float z)
		{
			return X * x - Y * y - Z * z;
		}

		#endregion

		#region Vector2 Params

		public Vector3 Set2(IReadOnlyVector2 vector)
		{
			return Set2(vector.X, vector.Y);
		}

		public Vector3 Add2(IReadOnlyVector2 vector)
		{
			return Add2(vector.X, vector.Y);
		}

		public Vector3 Subtract2(IReadOnlyVector2 vector)
		{
			return Subtract2(vector.X, vector.Y);
		}

		public Vector3 Multiply2(IReadOnlyVector2 vector)
		{
			return Multiply2(vector.X, vector.Y);
		}

		public Vector3 Divide2(IReadOnlyVector2 vector)
		{
			return Divide2(vector.X, vector.Y);
		}

		public float Dot2(IReadOnlyVector2 vector)
		{
			return Dot2(vector.X, vector.Y);
		}

		public float Cross2(IReadOnlyVector2 vector)
		{
			return Cross2(vector.X, vector.Y);
		}

		public Vector3 Normalize(float length = 1)
		{
			return Multiply(Math.Abs(length) / Length2);
		}

		public Vector3 Reflect2(IReadOnlyVector2 vector)
		{
			Vector2 normal = new Vector2(vector).Normalize2();
			return Reflect2(normal.X, normal.Y);
		}

		public Vector3 PerpendicularCCW2()
		{
			return Set2(Y, -X);
		}

		public Vector3 PerpendicularCW2()
		{
			return Set2(-Y, X);
		}

		#endregion

		#region Vector3 - Vectors

		public Vector3 Set3(IReadOnlyVector3 vector)
		{
			return Set3(vector.X, vector.Y, vector.Z);
		}

		public Vector3 Add3(IReadOnlyVector3 vector)
		{
			return Add3(vector.X, vector.Y, vector.Z);
		}

		public Vector3 Subtract3(IReadOnlyVector3 vector)
		{
			return Subtract3(vector.X, vector.Y, vector.Z);
		}

		public Vector3 Multiply3(IReadOnlyVector3 vector)
		{
			return Multiply3(vector.X, vector.Y, vector.Z);
		}

		public Vector3 Divide3(IReadOnlyVector3 vector)
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

		public Vector3 Normalize3(float length = 1)
		{
			float ratio = Math.Abs(length) / Length3;
			return Multiply3(ratio, ratio, ratio);
		}

		public Vector3 Reflect3(IReadOnlyVector3 vector)
		{
			Vector3 normal = new Vector3(vector).Normalize3();
			return Reflect3(normal.X, normal.Y, normal.Z);
		}

		public Vector3 RotateAround2(IReadOnlyVector2 vector, float radians)
		{
			float cosine = (float)Math.Cos(radians);
			float sine = (float)Math.Sin(radians);
			float x = X;
			float y = Y;
			x -= vector.X;
			y -= vector.Y;
			float offsetX = x * cosine - y * sine;
			float offsetY = x * sine + y * cosine;
			X = vector.X + offsetX;
			Y = vector.Y + offsetY;
			return this;
		}

		#endregion
	}
}