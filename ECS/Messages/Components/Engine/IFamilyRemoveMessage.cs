using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.Core.Messages
{
	public interface IFamilyRemoveMessage : IKeyValueMessage<IEngine, Type, IReadOnlyFamily>
	{

	}
}
