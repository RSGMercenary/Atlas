using Atlas.Engine.Components;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Messages
{
	public interface IFamilyRemoveMessage : IKeyValueMessage<IEngine, Type, IFamily>
	{

	}
}
