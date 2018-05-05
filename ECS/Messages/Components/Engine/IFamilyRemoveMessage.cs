using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.Framework.Messages
{
	public interface IFamilyRemoveMessage : IKeyValueMessage<IEngine, Type, IReadOnlyFamily>
	{

	}
}
