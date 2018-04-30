using Atlas.Engine.Collections.EngineList;
using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Messages;
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
			new EngineList<int>();

			//engine.AddSystemType<ITestSystem, TestSystem>();

			var fam = engine.Families;

			root.AddSystem<ITestSystem>();

			engine.AddSystemType<ITestSystem, TestSystem2>();

			engine.AddListener<IEntityAddMessage>(ListenFor1_1);

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name = index1.ToString();
				var depth1 = new AtlasEntity(name, name);
				for(int index2 = 1; index2 <= 5; ++index2)
				{
					name = index1 + "-" + index2;
					var depth2 = depth1.AddChild(name, name);

					/*for(int index3 = 1; index3 <= 5; ++index3)
					{
						name = index1 + "-" + index2 + "-" + index3;
						var depth3 = depth2.AddChild(name, name);
					}*/
				}
				root.AddChild(depth1);
			}

			//Make sure these values are always the same.
			Debug.WriteLine(engine.Entities);
			Debug.WriteLine(root.DescendantsToString());

			engine.IsRunning = true;
		}

		static private void ListenFor1_1(IEntityAddMessage message)
		{
			if(message.Value.GlobalName == "1-1")
			{
				message.Value.Parent.RemoveChild("1-3");
			}
		}
	}
}