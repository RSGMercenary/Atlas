using System;

namespace Atlas.Framework.Geometry
{
	class Vector2:Vector2<Vector2>
	{
		public static readonly IReadOnlyVector2 Up = new Vector2(0, 1);
		public static readonly IReadOnlyVector2 Down = new Vector2(0, -1);
		public static readonly IReadOnlyVector2 Left = new Vector2(-1, 0);
		public static readonly IReadOnlyVector2 Right = new Vector2(0, 1);

		public Vector2()
		{

		}

		public Vector2(float x = 0, float y = 0) : base(x, y)
		{

		}

		public Vector2(IReadOnlyVector2 vector) : base(vector)
		{

		}
	}

	class Vector2<TReturn>:IVector2<TReturn> where TReturn : Vector2<TReturn>
	{
		private float x = 0;
		private float y = 0;

		public Vector2()
		{

		}

		public Vector2(float x = 0, float y = 0)
		{
			Set2(x, y);
		}

		public Vector2(IReadOnlyVector2 vector)
		{
			Set2(vector);
		}

		override public string ToString()
		{
			return "(" + X + ", " + Y + ")";
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

		public float LengthSquared2
		{
			get { return X * X + Y * Y; }
		}

		public float Length2
		{
			get { return (float)Math.Sqrt(LengthSquared2); }
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

		#region Vector

		public virtual TReturn Set(float value)
		{
			return Set2(value, value);
		}

		public virtual TReturn Add(float value)
		{
			return Add2(value, value);
		}

		public virtual TReturn Subtract(float value)
		{
			return Subtract2(value, value);
		}

		public virtual TReturn Multiply(float value)
		{
			return Multiply2(value, value);
		}

		public virtual TReturn Divide(float value)
		{
			return Divide2(value, value);
		}

		#endregion

		#region Vector2 Float Params

		public TReturn Set2(float x, float y)
		{
			X = x;
			Y = y;
			return this as TReturn;
		}

		public TReturn Add2(float x = 0, float y = 0)
		{
			X += x;
			Y += y;
			return this as TReturn;
		}

		public TReturn Subtract2(float x = 0, float y = 0)
		{
			X -= x;
			Y -= y;
			return this as TReturn;
		}

		public TReturn Multiply2(float x = 1, float y = 1)
		{
			X *= x;
			Y *= y;
			return this as TReturn;
		}

		public TReturn Divide2(float x = 1, float y = 1)
		{
			X /= x;
			Y /= y;
			return this as TReturn;
		}

		public TReturn Rotate2(float radians)
		{
			return RotateAround2(0, 0, radians);
		}

		public TReturn RotateAround2(float x, float y, float radians)
		{
			double cos = Math.Cos(radians);
			double sin = -Math.Sin(radians); //Setting this negative seems to rotate things clockwise.
			double deltaX = X - x;
			double deltaY = Y - y;
			double offsetX = deltaX * cos - deltaY * sin;
			double offsetY = deltaX * sin + deltaY * cos;
			X = x + (float)offsetX;
			Y = y + (float)offsetY;
			return this as TReturn;
		}

		public TReturn Reflect2(float normalX, float normalY)
		{
			//return ReflectAround2(0, 0, normalX, normalY);
			float dot = Dot2(normalX, normalY);
			X -= dot * normalX;
			Y -= dot * normalY;
			return this as TReturn;
		}

		public TReturn ReflectAround2(float x, float y, float normalX, float normalY)
		{
			//We rotate perpendicular CCW to actually make a normal facing.
			float temp = normalX;
			normalX = normalY;
			normalY = -temp;
			x = X - x;
			y = Y - y;
			float dot = 2 * (x * normalX + y * normalY);
			X -= dot * normalX;
			Y -= dot * normalY;
			return this as TReturn;
		}

		public float Dot2(float x, float y)
		{
			return X * x + Y * y;
		}

		public float Cross2(float x, float y)
		{
			return X * x - Y * y;
		}

		public float DistanceSquared2(float x, float y)
		{
			float deltaX = X - x;
			float deltaY = Y - y;
			return deltaX * deltaX + deltaY * deltaY;
		}

		public float Distance2(float x, float y)
		{
			return (float)Math.Sqrt(DistanceSquared2(x, y));
		}

		public TReturn Normalize2(float length = 1)
		{
			//Should check for slight differences from 1.
			/*double min = 1 - 1e-14;
			double max = 1 + 1e-14;
			float lengthSquared2 = LengthSquared2;
			if(lengthSquared2 >= min && lengthSquared2 <= max)
				return this;*/

			float ratio = Math.Abs(length) / Length2;
			return Multiply2(ratio, ratio);
		}

		public TReturn PerpendicularCCW2()
		{
			return Set2(Y, -X);
		}

		public TReturn PerpendicularCW2()
		{
			return Set2(-Y, X);
		}

		#endregion

		#region Vector2 IReadOnlyVector2 Params

		public TReturn Set2(IReadOnlyVector2 vector)
		{
			return Set2(vector.X, vector.Y);
		}

		public TReturn Add2(IReadOnlyVector2 vector)
		{
			return Add2(vector.X, vector.Y);
		}

		public TReturn Subtract2(IReadOnlyVector2 vector)
		{
			return Subtract2(vector.X, vector.Y);
		}

		public TReturn Multiply2(IReadOnlyVector2 vector)
		{
			return Multiply2(vector.X, vector.Y);
		}

		public TReturn Divide2(IReadOnlyVector2 vector)
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

		public float DistanceSquared2(IReadOnlyVector2 vector)
		{
			return DistanceSquared2(vector.X, vector.Y);
		}

		public float Distance2(IReadOnlyVector2 vector)
		{
			return Distance2(vector.X, vector.Y);
		}

		public TReturn Reflect2(IReadOnlyVector2 vector)
		{
			Vector2 normal = new Vector2(vector).Normalize2();
			return Reflect2(normal.X, normal.Y);
		}

		public TReturn RotateAround2(IReadOnlyVector2 vector, float radians)
		{
			return RotateAround2(vector.X, vector.Y, radians);
		}

		public TReturn ReflectAround2(IReadOnlyVector2 vector, IReadOnlyVector2 normal)
		{
			Vector2 normalized = new Vector2(normal).Normalize2();
			return ReflectAround2(vector.X, vector.Y, normalized.X, normalized.Y);
		}

		#endregion
	}
}
