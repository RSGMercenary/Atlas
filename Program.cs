using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Testing.Systems;
using System.Diagnostics;

namespace Atlas
{
	class Program
	{
		private static void OnManagerAdded(IComponent engine, IEntity entity, int index)
		{

		}

		static void Main(string[] args)
		{
			IEntity root = AtlasEntity.Instance;
			IEngine engine = AtlasEngine.Instance;

			engine.ManagerAdded.Add(OnManagerAdded);

			root.AddComponent<IEngine>(engine);
			root.AddSystem<TestSystem>();

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = root.AddChild(new AtlasEntity(name1, name1));
				//child1.AddComponent<TestBuilder>();
				for(int index2 = 1; index2 <= 5; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					IEntity child2 = child1.AddChild(new AtlasEntity(name2, name2));
					//child2.AddComponent<TestComponent>();
				}
			}

			Debug.WriteLine(root.ToString(-1, true, false, false));

			//root.GetComponent<IEngine>().Run();
		}
	}
}