using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Testing.Components;
using Atlas.Testing.Systems;
using System.Diagnostics;

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

			for(int index1 = 1; index1 <= 10; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = entity.AddChild(new AtlasEntity(name1, name1));
				child1.AddComponent<TestComponent>();
				for(int index2 = 1; index2 <= 10; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					IEntity child2 = child1.AddChild(new AtlasEntity(name2, name2));
				}
			}

			Debug.WriteLine(entity.Dump());
			//Debug.WriteLine(entity.EntityManager.GetEntityByUniqueName("0-3-4").Dump());

			IEngineManager engine1 = engine;
			IComponent component1 = engine;


			Debug.WriteLine(engine1.Managers.Count);
			Debug.WriteLine(component1.Managers.Count);
			//TestSignals();
			while(true)
			{
				engine.Update();
			}
		}

		private static void TestSignals()
		{
			Debug.WriteLine("---------------------------");
			IEntity parent = new AtlasEntity();
			for(int index = 0; index < 10; ++index)
			{
				IEntity child = parent.AddChild(new AtlasEntity("", "Child" + index));
				child.ParentIndexChanged.Add(ParentIndexChanged);
			}

			parent.SetChildIndex(parent.GetChild(6), 2);
		}

		private static void ParentIndexChanged(IEntity entity, int next, int previous)
		{
			Debug.WriteLine(entity.LocalName + ", Next = " + next + ", Prev = " + previous);
		}
	}
}
