namespace Atlas.Framework.Geometry
{
	interface IReadOnlyVector2
	{
		float X { get; }
		float Y { get; }
		float LengthSquared2 { get; }
		float Length2 { get; }

		float RadiansZ { get; }
		float DegreesZ { get; }

		float Dot2(float x, float y);
		float Cross2(float x, float y);
		//float Radians(float x, float y);

		float Dot2(IReadOnlyVector2 vector);
		float Cross2(IReadOnlyVector2 vector);
	}

	interface IReadOnlyVector3:IReadOnlyVector2
	{
		float Z { get; }
		float LengthSquared3 { get; }
		float Length3 { get; }

		float Dot3(float x, float y, float z);
		float Cross3(float x, float y, float z);

		float Dot3(IReadOnlyVector3 vector);
		float Cross3(IReadOnlyVector3 vector);
	}

	interface IReadOnlyVector4:IReadOnlyVector3
	{
		float W { get; }
	}

	interface IVector2:IReadOnlyVector2
	{
		new float X { get; set; }
		new float Y { get; set; }
	}

	interface IVector3:IVector2, IReadOnlyVector3
	{
		new float Z { get; set; }
	}

	interface IVector4:IVector3, IReadOnlyVector4
	{
		new float W { get; set; }
	}

	interface IVector<TReturn>
	{
		TReturn Set(float value);
		TReturn Add(float value);
		TReturn Subtract(float value);
		TReturn Multiply(float value);
		TReturn Divide(float value);
	}

	interface IVector2<TReturn>:IVector<TReturn>, IVector2
	{
		TReturn Set2(float x, float y);
		TReturn Add2(float x = 0, float y = 0);
		TReturn Subtract2(float x = 0, float y = 0);
		TReturn Multiply2(float x = 1, float y = 1);
		TReturn Divide2(float x = 1, float y = 1);
		TReturn Reflect2(float x, float y);
		TReturn Normalize2(float length = 1);

		TReturn Set2(IReadOnlyVector2 vector);
		TReturn Add2(IReadOnlyVector2 vector);
		TReturn Subtract2(IReadOnlyVector2 vector);
		TReturn Multiply2(IReadOnlyVector2 vector);
		TReturn Divide2(IReadOnlyVector2 vector);
		TReturn Reflect2(IReadOnlyVector2 plane);

		TReturn RotateAround2(IReadOnlyVector2 vector, float radians);
		//TReturn ReflectAround2(IReadOnlyVector2 vector, IReadOnlyVector2 plane);

		TReturn PerpendicularCCW2();
		TReturn PerpendicularCW2();
	}

	interface IVector3<TReturn>:IVector2<TReturn>, IVector3
	{
		TReturn Set3(float x, float y, float z);
		TReturn Add3(float x = 0, float y = 0, float z = 0);
		TReturn Subtract3(float x = 0, float y = 0, float z = 0);
		TReturn Multiply3(float x = 1, float y = 1, float z = 1);
		TReturn Divide3(float x = 1, float y = 1, float z = 1);
		TReturn Normalize3(float length = 1);

		TReturn Set3(IReadOnlyVector3 vector);
		TReturn Add3(IReadOnlyVector3 vector);
		TReturn Subtract3(IReadOnlyVector3 vector);
		TReturn Multiply3(IReadOnlyVector3 vector);
		TReturn Divide3(IReadOnlyVector3 vector);
		TReturn Reflect3(IReadOnlyVector3 vector);
	}

	interface IVector4<TReturn>:IVector3<TReturn>, IVector4
	{
		TReturn Set4(float x, float y, float z, float w);
		TReturn Add4(float x = 0, float y = 0, float z = 0, float w = 0);
		TReturn Subtract4(float x = 0, float y = 0, float z = 0, float w = 0);
		TReturn Multiply4(float x = 1, float y = 1, float z = 1, float w = 1);
		TReturn Divide4(float x = 1, float y = 1, float z = 1, float w = 1);

		TReturn Set4(IVector4 vector);
		TReturn Add4(IVector4 vector);
		TReturn Subtract4(IVector4 vector);
		TReturn Multiply4(IVector4 vector);
		TReturn Divide4(IVector4 vector);
	}
}
