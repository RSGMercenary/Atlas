using Atlas.Engine;
using Atlas.Entities;
using Atlas.Families;
using Atlas.LinkList;
using Atlas.Systems;
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
				if(current.Value.GlobalName == "0-6-0")
				{
					current.Value.RemoveComponent<TestComponent>();
				}

			}
		}

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			tests = engine.AddFamily<TestFamily>();
		}

		protected override void RemovingEngine(IEngine engine)
		{
			engine.RemoveFamily<TestFamily>();
			tests = null;
			base.RemovingEngine(engine);
		}
	}
}
