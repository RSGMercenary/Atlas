using Atlas.Entities;
using Atlas.LinkList;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas
{
	class Program
	{
		static void Main(string[] args)
		{
			LinkedList<int> l = new LinkedList<int>();

			ILinkList<string> list = new LinkList<string>();

			list.Add("1");
			list.Add("2");
			list.Add("3");
			list.Add("4");
			list.Add("5");
			list.Add("6");
			list.SetIndex("3", 4);
			/*list.Remove("4");
			list.Remove("6");
			list.Remove("1");
			list.Remove("3");
			list.Remove("2");
			list.Remove("5");
			list.Remove("7");
			*/
			Debug.WriteLine(list.ToString());
			Debug.WriteLine("Forwards");
			for(ILinkListNode<string> current = list.First; current != null; current = current.Next)
			{
				Debug.WriteLine(current.Value);
			}
			Debug.WriteLine("Backwards");
			for(ILinkListNode<string> current = list.Last; current != null; current = current.Previous)
			{
				Debug.WriteLine(current.Value);
			}
			return;

			string name = "0-0-0";
			IEntity entity = new Entity(name, name);
			entity.AddComponent(EntityManager.Instance);

			for(int index1 = 1; index1 <= 10; ++index1)
			{
				string name1 = "0-" + index1 + "-0";
				IEntity child1 = entity.AddChild(new Entity(name1, name1));
				for(int index2 = 1; index2 <= 10; ++index2)
				{
					string name2 = "0-" + index1 + "-" + index2;
					IEntity child2 = child1.AddChild(new Entity(name2, name2));
				}
			}

			Debug.WriteLine(entity.Dump());
			//Debug.WriteLine(entity.EntityManager.GetEntityByUniqueName("0-3-4").Dump());

			//TestSignals();
		}

		private static void TestSignals()
		{
			Debug.WriteLine("---------------------------");
			IEntity parent = new Entity();
			for(int index = 0; index < 10; ++index)
			{
				IEntity child = parent.AddChild(new Entity("", "Child" + index));
				child.ParentIndexChanged.Add(ParentIndexChanged);
			}

			parent.SetChildIndex(parent.GetChild(6), 2);
		}

		private static void ParentIndexChanged(IEntity entity, int next, int previous)
		{
			Debug.WriteLine(entity.LocalName + ", Next = " + next + ", Prev = " + previous);
		}
	}
}
