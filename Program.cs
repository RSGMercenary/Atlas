using Atlas.Entities;
using Atlas.Signals;
using System;
using System.Diagnostics;

namespace Atlas
{
	class Program
	{
		private static string name = "";
		private static Signal nameChanged = new Signal(false);

		private static void Testing()
		{
			nameChanged.Add((Action<Entity, string, string>)NameChanged1);
			nameChanged.Add((Action<Entity, string, string>)NameChanged2);
			nameChanged.Add((Action<Entity, string, string>)NameChanged3);
			Name = "Drew";
		}

		public static string Name
		{
			get
			{
				return name;
			}
			set
			{
				if(name != value && value != null)
				{
					string previous = name;
					name = value;
					nameChanged.Dispatch(null, value, previous);
				}
			}
		}

		private static void NameChanged1(Entity entity, string current, string previous)
		{
			Debug.WriteLine("1");
			if(current == "Drew")
			{
				Name = "Not Drew";
			}
		}

		private static void NameChanged2(Entity entity, string current, string previous)
		{
			Debug.WriteLine("2");
			if(current == "Not Drew")
			{
				Name = "Really Not Drew";
			}
		}

		private static void NameChanged3(Entity entity, string current, string previous)
		{
			Debug.WriteLine("3");
			if(current == "Really Not Drew")
			{
				//Name = "Really Not Drew";
			}
		}


		static void Main(string[] args)
		{
			Testing();

			return;

			string name = "0-0-0";
			Entity entity = new Entity(name, name);
			entity.AddComponent(EntityManager.Instance);

			for(int index1 = 1; index1 <= 10; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				Entity child1 = entity.AddChild(new Entity(name1, name1));
				for(int index2 = 1; index2 <= 10; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					Entity child2 = child1.AddChild(new Entity(name2, name2));
				}
			}

			Debug.WriteLine(entity.Dump());
			Debug.WriteLine(entity.EntityManager.GetEntityByUniqueName("0-3-4").Dump());

			//TestSignals();
		}

		private static void TestSignals()
		{
			Debug.WriteLine("---------------------------");
			Entity parent = new Entity();
			for(int index = 0; index < 10; ++index)
			{
				Entity child = parent.AddChild(new Entity("", "Child" + index));
				child.ParentIndexChanged.Add(ParentIndexChanged);
			}

			parent.SetChildIndex(parent.GetChild(6), 2);
		}

		private static void ParentIndexChanged(Entity entity, int next, int previous)
		{
			Debug.WriteLine(entity.Name + ", Next = " + next + ", Prev = " + previous);
		}
	}
}
