using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Testing.Components;
using Atlas.Testing.Systems;
using System.Diagnostics;

namespace Atlas
{
	class Program
	{
		int count = 0;

		static void Main(string[] args)
		{
			string name = "0-0-0";
			IEntity root = new AtlasEntity(name, name);
			root.AddSystem<TestSystem>();
			IEngine engine = root.AddComponent<AtlasEngine, IEngine>(AtlasEngine.Instance);

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = root.AddChild(new AtlasEntity(name1, name1));
				child1.AddComponent<TestComponent>();
				child1.AddSystem<TestSystem>();
				for(int index2 = 1; index2 <= 5; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					IEntity child2 = child1.AddChild(new AtlasEntity(name2, name2));
					//child2.AddComponent<TestComponent>();
				}
			}

			root.AddChild();
			root.AddChild();

			Debug.WriteLine(root.ToString());

			engine.Run();

			//Will never get here now. Run is infinite.
			root.Dispose();
			Debug.WriteLine("=== Done ===");
			Debug.WriteLine(root.ToString());
			Debug.WriteLine(engine.ToString());
		}
	}
}