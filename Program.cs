using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Testing.Components;
using Atlas.Testing.Systems;

namespace Atlas
{
	class Program
	{
		static void Main(string[] args)
		{
			string name = "0-0-0";
			IEntity entity = new AtlasEntity(name, name);
			entity.AddSystem<TestSystem>();
			IEngineManager engine = entity.AddComponent<AtlasEngineManager, IEngineManager>(AtlasEngineManager.Instance);

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

			while(true)
			{
				engine.Update();
			}
		}
	}
}
