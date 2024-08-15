using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace Atlas.Tests.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
abstract class TestCaseGenericAttribute : TestCaseAttribute, ITestBuilder
{
	private Type[] GenericTypeArgs { get; init; }

	protected TestCaseGenericAttribute(Type[] genericTypeArgs, params object[] arguments) : base(arguments) => GenericTypeArgs = genericTypeArgs;

	IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
	{
		if(!method.IsGenericMethodDefinition)
			return BuildFrom(method, suite);

		if(GenericTypeArgs?.Length != method.GetGenericArguments().Length)
		{
			var parameters = new TestCaseParameters { RunState = RunState.NotRunnable };
			parameters.Properties.Set(PropertyNames.SkipReason, $"{nameof(GenericTypeArgs)} should have {method.GetGenericArguments().Length} elements");
			return new[] { new NUnitTestCaseBuilder().BuildTestMethod(method, suite, parameters) };
		}

		return BuildFrom(method.MakeGenericMethod(GenericTypeArgs), suite);
	}
}