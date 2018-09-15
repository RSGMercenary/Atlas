﻿using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Messages
{
	public interface IEngineMessage : IPropertyMessage<IEngineObject, IEngine>
	{
	}
}