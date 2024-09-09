using Atlas.ECS.Components.Engine;
using Atlas.ECS.Families;
using Newtonsoft.Json;

namespace Atlas.ECS.Systems;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class AtlasFamilySystem<TFamilyMember> : AtlasSystem, IFamilySystem<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
{
	[JsonProperty(Order = int.MaxValue)]
	public IReadOnlyFamily<TFamilyMember> Family { get; private set; }

	[JsonProperty]
	public bool IgnoreSleep { get; protected set; } = AtlasECS.IgnoreSleep;

	protected override void SystemUpdate(float deltaTime)
	{
		var ignoreSleep = IgnoreSleep;
		foreach(var member in Family)
		{
			if(ignoreSleep || !member.Entity.IsSleeping)
				MemberUpdate(deltaTime, member);
		}
	}

	protected virtual void MemberUpdate(float deltaTime, TFamilyMember member) { }

	protected virtual void MemberAdded(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

	protected virtual void MemberRemoved(IReadOnlyFamily<TFamilyMember> family, TFamilyMember member) { }

	protected override void AddingEngine(IEngine engine)
	{
		Family = engine.Families.Add<TFamilyMember>();
		foreach(var member in Family)
			MemberAdded(Family, member);
		Family.MemberAdded += MemberAdded;
		Family.MemberRemoved += MemberRemoved;
	}

	protected override void RemovingEngine(IEngine engine)
	{
		Family.MemberAdded -= MemberAdded;
		Family.MemberRemoved -= MemberRemoved;
		foreach(var member in Family)
			MemberRemoved(Family, member);
		engine.Families.Remove<TFamilyMember>();
		Family = null;
	}
}