﻿using Atlas.ECS.Components;
using Atlas.Framework.Messages;
using Atlas.Framework.Objects;

namespace Atlas.ECS.Objects
{
	public abstract class EngineObject : AtlasObject, IEngineObject
	{
		private IEngine engine;

		public virtual IEngine Engine
		{
			get { return engine; }
			set
			{
				if(engine == value)
					return;
				var previous = engine;
				engine = value;
				Message<IEngineMessage>(new EngineMessage(value, previous));
			}
		}
	}
}