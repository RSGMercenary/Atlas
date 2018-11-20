using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Components.Messages
{
	public interface ISystemTypeRemoveMessage : IValueMessage<ISystemManager, Type>
	{

	}
}
