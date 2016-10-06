using Atlas.Interfaces.Entities;
using System;
using System.Collections.Generic;

namespace Atlas.Interfaces.Components
{
	interface IComponent
	{
		List<IEntity> ComponentManagers
		{
			get;
		}

		int ComponentManagerCount
		{
			get;
		}

		IEntity GetEntity(int index);

		IEntity AddEntity(IEntity entity);
		IEntity AddEntity(IEntity entity, Type type);
		IEntity AddEntity(IEntity entity, Type type, int index);

		IEntity RemoveEntity(IEntity entity);
		IEntity RemoveEntity(IEntity entity, Type type);
		IEntity RemoveEntity(int index);
	}
}
