using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.ECS.Messages
{
	public interface IFamilyAddMessage : IKeyValueMessage<IEngine, Type, IReadOnlyFamily>
	{

	}
}
