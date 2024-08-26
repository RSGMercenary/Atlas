using Atlas.ECS.Components.Engine;
using Atlas.ECS.Families;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.ECS.Systems;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class AtlasFamilySystem<TFamilyMember> : AtlasSystem, IFamilySystem<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
{
	[JsonProperty(Order = int.MaxValue)]
	public IReadOnlyFamily<TFamilyMember> Family { get; private set; }

	[JsonProperty]
	public bool UpdateSleepingEntities { get; protected set; } = false;

	protected override void SystemUpdate(float deltaTime)
	{
		var updateSleepingEntities = UpdateSleepingEntities;
		foreach(var member in Family)
		{
			if(updateSleepingEntities || !member.Entity.IsSleeping)
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

	public IEnumerator<TFamilyMember> GetEnumerator() => Family?.GetEnumerator() ?? Enumerable.Empty<TFamilyMember>().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}