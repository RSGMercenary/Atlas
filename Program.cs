using Atlas.Engine.Entities;
using Atlas.Framework.Geometry;
using Atlas.Testing.Components;
using Atlas.Testing.Systems;
using System.Diagnostics;

namespace Atlas
{
	class Program
	{
		static void Main(string[] args)
		{

			Vector2 vector1 = new Vector2(6, 5);
			Vector2 vector2 = new Vector2(6, 5);

			//float degrees = 90;
			//float radians = degrees * (float)(Math.PI / 180);
			vector1.ReflectAround2(new Vector2(7, 5), new Vector2(1, 0));
			vector2.Reflect2(new Vector2(1, -1));

			IEngine root = AtlasEngine.Instance;
			root.AddSystemType<TestSystem>();

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = root.AddChild(new AtlasEntity(name1, name1));
				child1.AddComponent<TestComponent>();
				child1.AddSystemType<TestSystem>();
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

			root.Run();

			//Will never get here now. Run is infinite.
			root.Dispose();
			Debug.WriteLine("=== Done ===");
			Debug.WriteLine(root.ToString());
			//Debug.WriteLine(engine.ToString());
		}
	}
}