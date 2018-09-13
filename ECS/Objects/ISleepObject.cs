﻿using Atlas.Core.Objects;

namespace Atlas.ECS.Objects
{
	public interface ISleepObject : IObject
	{
		bool IsSleeping { get; set; }
		int Sleeping { get; }
	}
}
