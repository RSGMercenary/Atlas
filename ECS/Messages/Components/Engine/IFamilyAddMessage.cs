using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.Core.Messages
{
	public interface IFamilyAddMessage : IKeyValueMessage<IEngine, Type, IReadOnlyFamily>
	{

	}
}
