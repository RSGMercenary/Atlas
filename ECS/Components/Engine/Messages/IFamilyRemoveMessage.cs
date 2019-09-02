using Atlas.Core.Messages;
using Atlas.ECS.Families;
using System;

namespace Atlas.ECS.Components.Messages
{
	public interface IFamilyRemoveMessage : IKeyValueMessage<IEngine, Type, IFamily> { }
}