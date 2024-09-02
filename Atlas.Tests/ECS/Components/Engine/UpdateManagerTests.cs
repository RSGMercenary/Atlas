using Atlas.ECS.Components.Engine;
using Atlas.Tests.Testers.Utilities;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Components.Engine;

[TestFixture]
internal class UpdateManagerTests
{
	public AtlasEngine Engine;

	[SetUp]
	public void SetUp()
	{
		Engine = new AtlasEngine();
	}

	[TestCase(30, 0)]
	[TestCase(16, 1)]
	[TestCase(13, 2)]
	[TestCase(11, 3)]
	[TestCase(10, 4)]
	public void When_Update_Then_FixedLagExpected(float fps, int expected)
	{
		Engine.Updates.Update(1 / fps);
		Engine.Updates.Update(TestFps._60);

		Assert.That(Engine.Updates.FixedLag == expected);
	}

	[TestCase(TestFps._0, TestFps._0)]
	[TestCase(TestFps._30, TestFps._60)]
	[TestCase(TestFps._60, TestFps._120)]
	[TestCase(TestFps._120, TestFps._120)]
	[TestCase(TestFps._120, TestFps._60)]
	[TestCase(TestFps._60, TestFps._30)]
	[TestCase(0.25f, TestFps._1)]
	public void When_Update_Then_DeltaVariableTimeExpected(float maxVariableTime, float deltaVariableTime)
	{
		Engine.Updates.MaxVariableTime = maxVariableTime;

		Engine.Updates.Update(deltaVariableTime);

		Assert.That(Engine.Updates.DeltaVariableTime == float.MinNumber(maxVariableTime, deltaVariableTime));
	}

	[TestCase(TestFps._0, TestFps._0, 0f)]
	[TestCase(TestFps._60, TestFps._120, 0.5f)]
	[TestCase(TestFps._30, TestFps._120, 0.25f)]
	[TestCase(TestFps._30, TestFps._60, 0.5f)]
	[TestCase(TestFps._15, TestFps._60, 0.25f)]
	[TestCase(TestFps._15, TestFps._120, 0.125f)]
	public void When_Update_Then_VariableInterpolationExpected(float deltaFixedTime, float deltaTime, float variableInterpolation)
	{
		Engine.Updates.DeltaFixedTime = deltaFixedTime;
		Engine.Updates.MaxVariableTime = deltaTime;

		Engine.Updates.Update(deltaTime);

		Assert.That(Engine.Updates.VariableInterpolation == variableInterpolation);
	}

	[TestCase(TestFps._0)]
	[TestCase(TestFps._30)]
	[TestCase(TestFps._60)]
	[TestCase(TestFps._120)]
	[TestCase(TestFps._1)]
	public void When_MaxVariableTime_Then_MaxVariableTimeExpected(float maxVariableTime)
	{
		Engine.Updates.MaxVariableTime = maxVariableTime;

		Assert.That(Engine.Updates.MaxVariableTime == maxVariableTime);
	}

	[TestCase(TestFps._0)]
	[TestCase(TestFps._30)]
	[TestCase(TestFps._60)]
	[TestCase(TestFps._120)]
	[TestCase(TestFps._1)]
	public void When_DeltaFixedTime_Then_DeltaFixedTimeExpected(float deltaFixedTime)
	{
		Engine.Updates.DeltaFixedTime = deltaFixedTime;
		Engine.Updates.Update(deltaFixedTime);

		Assert.That(Engine.Updates.DeltaFixedTime == deltaFixedTime);
	}
}