using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using System;

namespace Atlas.Tests.ECS.Systems.Systems;

class TestSystem : AtlasSystem, ITestSystem
{
	public bool TestUpdate = false;
	public bool TestDispose = false;
	public bool BlockDispose = true;
	public Action TestAction;

	public override void Dispose()
	{
		if(BlockDispose)
			return;
		base.Dispose();
		TestDispose = true;
	}


	protected override void SystemUpdate(float deltaTime)
	{
		TestUpdate = true;
		TestAction?.Invoke();
	}

	public new float DeltaIntervalTime
	{
		get => base.DeltaIntervalTime;
		set => base.DeltaIntervalTime = value;
	}

	public new TimeStep UpdateStep
	{
		get => base.UpdateStep;
		set => base.UpdateStep = value;
	}

	public new TimeStep UpdateState
	{
		get => base.UpdateState;
		set => base.UpdateState = value;
	}
}