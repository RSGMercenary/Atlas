using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Systems;

[TestFixture]
public class SystemTests
{
	public TestSystem System;

	[SetUp]
	public void SetUp()
	{
		System = new TestSystem();
	}

	[TestCase(TestFps._0, false)]
	[TestCase(TestFps._30, true)]
	[TestCase(TestFps._60, true)]
	[TestCase(TestFps._120, true)]
	[TestCase(TestFps._1, true)]
	public void When_Update_Then_UpdateExpected(float deltaTime, bool expected)
	{
		var updated = false;

		System.AddListener<IUpdateStateMessage<ISystem>>(_ => updated = true);
		System.Update(deltaTime);

		Assert.That(updated == expected);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_Update_And_UpdateLock_Then_ThrowsExpected(bool updateLock)
	{
		var method = () => System.Update(TestFps._60);

		if(updateLock)
			System.AddListener<IUpdateStateMessage<ISystem>>(_ => method.Invoke());

		Assert.That(method, updateLock ? Throws.Exception : Throws.Nothing);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_IsSleeping_Then_IsSleepingExpected(bool isSleeping)
	{
		var updated = false;

		System.IsSleeping = isSleeping;
		System.AddListener<IUpdateStateMessage<ISystem>>(_ => updated = true);
		System.Update(1);

		Assert.That(updated == !isSleeping);
		Assert.That(System.IsSleeping == isSleeping);
		Assert.That(System.Sleeping == (isSleeping ? 1 : -1));
	}

	[TestCase(TimeStep.Fixed)]
	[TestCase(TimeStep.Variable)]
	[TestCase(TimeStep.None)]
	public void When_UpdateStep_Then_UpdateStepExpected(TimeStep updateStep)
	{
		System.UpdateStep = updateStep;

		Assert.That(System.UpdateStep == updateStep);
	}

	[TestCase(TimeStep.Fixed)]
	[TestCase(TimeStep.Variable)]
	[TestCase(TimeStep.None)]
	public void When_UpdateState_Then_UpdateStateExpected(TimeStep updateState)
	{
		System.UpdateState = updateState;

		Assert.That(System.UpdateState == updateState);
	}

	[TestCase(TestFps._0, 1, 0)]
	[TestCase(TestFps._1, 1, 0)]
	[TestCase(TestFps._30, 1, 0)]
	[TestCase(TestFps._60, 1, 0)]
	[TestCase(TestFps._120, 1, 0)]
	[TestCase(TestFps._30, 2, 0)]
	[TestCase(TestFps._60, 2, 0)]
	[TestCase(TestFps._120, 2, 0)]
	[TestCase(TestFps._30, 100, 0.00000192f)]
	[TestCase(TestFps._60, 100, 0.00000096f)]
	[TestCase(TestFps._120, 100, 0.00000048f)]
	[TestCase(TestFps._30, 1000, 0.000168f)]
	[TestCase(TestFps._60, 1000, 0.000084f)]
	[TestCase(TestFps._120, 1000, 0.000042f)]
	public void When_DeltaIntervalTime_Then_DeltaIntervalTimeExpected(float deltaIntervalTime, int count, float within)
	{
		System.DeltaIntervalTime = deltaIntervalTime;

		for(int i = 0; i < count; i++)
			System.Update(deltaIntervalTime);

		Assert.That(System.DeltaIntervalTime == deltaIntervalTime);
		Assert.That(System.TotalIntervalTime, Is.EqualTo(deltaIntervalTime * count).Within(within));
	}

	[TestCase(0)]
	[TestCase(2)]
	[TestCase(5)]
	[TestCase(7)]
	[TestCase(10)]
	public void When_Priority_Then_PriorityExpected(int priority)
	{
		System.Priority = priority;

		Assert.That(System.Priority == priority);
	}

	[Test]
	public void When_Dispose_Then_Disposed()
	{
		System.BlockDispose = false;
		System.Dispose();
		Assert.That(System.TestDispose);
	}
}