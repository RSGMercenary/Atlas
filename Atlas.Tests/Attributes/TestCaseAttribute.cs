namespace Atlas.Tests.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T> : TestCaseGenericAttribute
{
	public TestCaseAttribute(params object[] arguments) : base(new[] { typeof(T) }, arguments) { }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2> : TestCaseGenericAttribute
{
	public TestCaseAttribute(params object[] arguments) : base(new[] { typeof(T1), typeof(T2) }, arguments) { }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2, T3> : TestCaseGenericAttribute
{
	public TestCaseAttribute(params object[] arguments) : base(new[] { typeof(T1), typeof(T2), typeof(T3) }, arguments) { }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2, T3, T4> : TestCaseGenericAttribute
{
	public TestCaseAttribute(params object[] arguments) : base(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, arguments) { }
}