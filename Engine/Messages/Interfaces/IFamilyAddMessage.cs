using Atlas.Engine.Components;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Messages
{
	interface IFamilyAddMessage : IKeyValueMessage<IEngine, Type, IFamily>
	{

	}
}
