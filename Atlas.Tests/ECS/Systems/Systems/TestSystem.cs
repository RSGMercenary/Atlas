using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Systems;
using System.Diagnostics.CodeAnalysis;

namespace Atlas.Tests.ECS.Systems.Systems;

[ExcludeFromCodeCoverage]
public class TestSystem : AtlasSystem, ITestSystem
{
	public bool TestDispose = false;

	public bool BlockDispose = true;

	public TestSystem()
	{

	}

	public override void Dispose()
	{
		if(BlockDispose)
			return;
		base.Dispose();
		TestDispose = true;
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

	protected override void AddingEngine(IEngine engine)
	{

	}

	protected override void RemovingEngine(IEngine engine)
	{

	}
}