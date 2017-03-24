using Atlas.Engine.Components;
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
			IEntity root = AtlasEntity.Instance;
			IEngine engine = root.AddComponent<IEngine>(AtlasEngine.Instance);

			engine.AddSystemType<ITestSystem, TestSystem>();

			root.AddSystem<ITestSystem>();

			engine.AddSystemType<ITestSystem, TestSystem2>();

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = root.AddChild(new AtlasEntity(name1, name1));
				//child1.AddComponent<TestComponent>();
				child1.AddComponent<ITestComponent, TestComponent>();
				for(int index2 = 1; index2 <= 5; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					IEntity child2 = child1.AddChild(new AtlasEntity(name2, name2));
				}
			}

			Debug.WriteLine(root.ToString(-1, true, false, false));

			engine.Run();
		}
	}
}