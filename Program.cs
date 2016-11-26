using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
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
			IEntity root = new AtlasEntity(name, name);
			root.AddSystem<TestSystem>();
			IEngineManager engine = root.AddComponent<AtlasEngineManager, IEngineManager>(AtlasEngineManager.Instance);

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = root.AddChild(new AtlasEntity(name1, name1));
				child1.AddComponent<TestComponent>();
				for(int index2 = 1; index2 <= 5; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					IEntity child2 = child1.AddChild(new AtlasEntity(name2, name2));
					child2.AddComponent<TestComponent>();
				}
			}

			//Debug.WriteLine(entity.ToString(1, true, false));
			Debug.WriteLine(root.ToString(-1, false, false, true));

			return;
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
