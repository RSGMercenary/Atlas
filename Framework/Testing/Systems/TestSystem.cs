using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.LinkList;
using Atlas.Engine.Systems;
using Atlas.Testing.Components;
using Atlas.Testing.Families;
using System;
using System.Diagnostics;

namespace Atlas.Testing.Systems
{
	class TestSystem:AtlasSystem
	{
		private IFamily tests;

		protected override void Updating()
		{
			Debug.WriteLine("Updating " + Guid.NewGuid().ToString("N"));
			for(ILinkListNode<IEntity> current = tests.Entities.First; current != null; current = current.Next)
			{
				Debug.WriteLine(current.Value.GlobalName);
				if(current.Value.GlobalName == "0-3-0")
				{
					current.Value.RemoveComponent<TestComponent>();
				}
			}
		}

		protected override void AddingEngine(IEngineManager engine)
		{
			base.AddingEngine(engine);
			tests = engine.AddFamily<TestFamily>();
		}

		protected override void RemovingEngine(IEngineManager engine)
		{
			engine.RemoveFamily<TestFamily>();
			tests = null;
			base.RemovingEngine(engine);
		}
	}
}
