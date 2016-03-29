using System.Diagnostics;
using System.Collections.Generic;
using Atlas.Entities;
using Atlas.Signals;
using Atlas.Systems;
using System.Reflection;

namespace Atlas
{
    class Program
    {
        private static List<object> list = new List<object>();

        static void Main(string[] args)
        {
            string name = "0-0-0";
            Entity entity = new Entity(name, name);
            entity.AddComponent(new EntityManager());
            
            for(int index1 = 1; index1 <= 10; ++index1)
            {
                string name1 = "0-" + index1 + "-0";
                Entity child1 = entity.AddChild(new Entity(name1, name1));
                for (int index2 = 1; index2 <= 10; ++index2)
                {
                    string name2 = "0-" + index1 + "-" + index2;
                    Entity child2 = child1.AddChild(new Entity(name2, name2));
                }
            }

            Debug.WriteLine(entity.Dump());
            Debug.WriteLine(entity.EntityManager.GetEntityByUniqueName("0-3-4").Dump());
            
        }

        public void listenToThis(Entity entity, int index)
        {
            Debug.WriteLine("It's alive!");
        }
    }
}
