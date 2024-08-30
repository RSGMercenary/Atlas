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

	public new TimeStep TimeStep
	{
		get => base.TimeStep;
		set => base.TimeStep = value;
	}

	public new bool IsUpdating
	{
		get => base.IsUpdating;
		set => base.IsUpdating = value;
	}
}