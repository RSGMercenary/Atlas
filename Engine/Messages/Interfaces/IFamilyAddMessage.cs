using Atlas.Engine.Components;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Messages
{
	public interface IFamilyAddMessage : IKeyValueMessage<IEngine, Type, IFamily>
	{

	}
}
