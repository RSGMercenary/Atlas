using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using Atlas.Messages;
using Atlas.Testing.Components;
using Atlas.Testing.Systems;
using System.Diagnostics;

namespace Atlas
{
	class EntityMessage:Message<IEntity>, IEntityMessage
	{

	}

	interface IEntityMessage:IMessage<IEntity>
	{

	}

	class Program
	{
		static void Main(string[] args)
		{
			string name = "0-0-0";
			IEntity entity = new AtlasEntity(name, name);
			entity.AddSystem<TestSystem>();
			IEngineManager engine = entity.AddComponent<AtlasEngineManager, IEngineManager>(AtlasEngineManager.Instance);

			AtlasEntity ec = new AtlasEntity();
			IEntity ei = ec;

			SignalMessage<EntityMessage, IEntity> o = new SignalMessage<EntityMessage, IEntity>();
			ISignalMessage<EntityMessage, IEntity> m = o;
			m.Add(AcceptMessage);

			AcceptMessage(new Message<IEntity>());
			AcceptMessage(new EntityMessage());
			AcceptMessage(new PropertyMessage<IEntity, int>());
			Debug.WriteLine(m);
			Debug.WriteLine(m);

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = entity.AddChild(new AtlasEntity(name1, name1));
				child1.AddComponent<TestComponent>();
				for(int index2 = 1; index2 <= 5; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					IEntity child2 = child1.AddChild(new AtlasEntity(name2, name2));
				}
			}

			Debug.WriteLine(entity.ToString(true, false, false));

			while(true)
			{
				engine.Update();
			}
		}

		private static void AcceptMessage(IMessage<IEntity> message)
		{

		}
	}
}
