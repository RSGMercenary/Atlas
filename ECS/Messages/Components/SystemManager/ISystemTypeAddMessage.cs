using Atlas.Core.Messages;
using Atlas.ECS.Components;
using System;

namespace Atlas.ECS.Messages
{
	public interface ISystemTypeAddMessage : IValueMessage<ISystemManager, Type>
	{

	}
}
