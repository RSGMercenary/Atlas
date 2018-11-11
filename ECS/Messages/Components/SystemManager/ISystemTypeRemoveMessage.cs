using Atlas.Core.Messages;
using Atlas.ECS.Components;
using System;

namespace Atlas.ECS.Messages
{
	public interface ISystemTypeRemoveMessage : IValueMessage<ISystemManager, Type>
	{

	}
}
