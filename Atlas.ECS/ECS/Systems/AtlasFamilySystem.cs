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
		Family = engine.AddFamily<TFamilyMember>();
		foreach(var member in Family)
			MemberAdded(Family, member);
		Family.AddListener<IFamilyMemberAddMessage<TFamilyMember>>(MemberAdded);
		Family.AddListener<IFamilyMemberRemoveMessage<TFamilyMember>>(MemberRemoved);
	}

	protected override void RemovingEngine(IEngine engine)
	{
		Family.RemoveListener<IFamilyMemberAddMessage<TFamilyMember>>(MemberAdded);
		Family.RemoveListener<IFamilyMemberRemoveMessage<TFamilyMember>>(MemberRemoved);
		foreach(var member in Family)
			MemberRemoved(Family, member);
		engine.RemoveFamily<TFamilyMember>();
		Family = null;
	}

	private void MemberAdded(IFamilyMemberAddMessage<TFamilyMember> message) => MemberAdded(message.Messenger, message.Value);

	private void MemberRemoved(IFamilyMemberRemoveMessage<TFamilyMember> message) => MemberRemoved(message.Messenger, message.Value);

	public IEnumerator<TFamilyMember> GetEnumerator() => Family?.GetEnumerator() ?? Enumerable.Empty<TFamilyMember>().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}