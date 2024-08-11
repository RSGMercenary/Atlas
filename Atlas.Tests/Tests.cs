using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Serialization;
using Atlas.Tests.ECS.Components.Components;
using Atlas.Tests.ECS.Families.Families;
using Atlas.Tests.ECS.Systems.Systems;

namespace Atlas.Tests;

public static class Tests
{
	public static void Main()
	{
		var engine = new AtlasEngine();
		var system = engine.AddSystem<TestFamilySystem>();
		var family = engine.GetFamily<TestFamilyMember>();

		var component = new TestComponent(true);

		var root = new AtlasEntity(true);
		/*
		for(var i = 1; i <= 3; i++)
		{
			var entity = root.AddChild(new AtlasEntity("Child1-" + i));
			entity.AddComponent(component);

			for(var x = 1; x <= 3; x++)
			{
				entity.AddChild(new AtlasEntity("Child2-" + x));
			}

		}*/

		root.AddComponent<IEngine>(engine);
		root.AddComponent(component);

		//Console.WriteLine(family.Serialize(Newtonsoft.Json.Formatting.Indented));
		Console.WriteLine(root.Serialize(Newtonsoft.Json.Formatting.Indented));


		//var newEntity = EntitySerializer.Deserialize<IEntity>(json);
	}
}