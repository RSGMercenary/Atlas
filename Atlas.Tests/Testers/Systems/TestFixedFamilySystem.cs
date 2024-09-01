﻿using Atlas.Core.Objects.Update;
using Atlas.ECS.Entities;
using Atlas.ECS.Systems;
using Atlas.Tests.Testers.Components;
using Atlas.Tests.Testers.Families;

namespace Atlas.Tests.Testers.Systems;

class TestFixedFamilySystem : AtlasFamilySystem<TestFamilyMember>, ITestFixedFamilySystem
{
	public bool TestUpdate = false;
	public bool TestAddEntity = false;
	public bool TestRemoveEntity = false;
	public bool TestRemoveSystem = false;

	public TestFixedFamilySystem()
	{
		TimeStep = TimeStep.Fixed;
	}

	protected override void SystemUpdate(float deltaTime)
	{
		base.SystemUpdate(deltaTime);
		TestUpdate = true;
	}

	protected override void MemberUpdate(float deltaTime, TestFamilyMember member)
	{
		member.Component.TestUpdate = true;

		if(TestAddEntity)
		{
			var entity = new AtlasEntity();
			entity.AddComponent<TestComponent>();
			member.Entity.Parent.AddChild(entity);
		}

		if(TestRemoveEntity)
		{
			var entity = member.Entity;
			entity.Parent.RemoveChild(entity);
		}

		if(TestRemoveSystem)
			Engine.Systems.Remove<TestFixedFamilySystem>();
	}

	public new bool UpdateSleepingEntities
	{
		get => base.UpdateSleepingEntities;
		set => base.UpdateSleepingEntities = value;
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