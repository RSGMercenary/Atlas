using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Messages;
using System.Diagnostics;

namespace Atlas
{
	class Program
	{
		static void Main(string[] args)
		{
			IEntity root = AtlasEntity.Instance;
			IEngine engine = root.AddComponent<IEngine>(AtlasEngine.Instance);

			//engine.AddSystemType<ITestSystem, TestSystem>();

			//root.AddSystem<ITestSystem>();

			//engine.AddSystemType<ITestSystem, TestSystem2>();

			for(int index1 = 1; index1 <= 5; ++index1)
			{
				string name = index1.ToString();
				var depth1 = root.AddChild(name, name);
				depth1.AddListener(AtlasMessage.RemoveChild, OnRemove);
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
			}

			var ent = engine.GetEntity("1");
			ent.RemoveChild("1-2");

			Debug.WriteLine(engine.Entities);

			engine.IsRunning = true;
		}

		private static void OnRemove(IMessage<IEntity> message)
		{
			if(!message.AtTarget)
				return;
			var cast = (IKeyValueMessage<IEntity, int, IEntity>)message;
			if(cast.Value.LocalName == "1-2")
				message.Target.Parent = null;
		}
	}
}