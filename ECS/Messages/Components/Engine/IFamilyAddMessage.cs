using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.Framework.Messages
{
	public interface IFamilyAddMessage : IKeyValueMessage<IEngine, Type, IReadOnlyFamily>
	{

	}
}
