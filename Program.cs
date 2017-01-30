using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
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
            LinkList<int> list = new LinkList<int>();
            list.Add(1, 2, 3, 4, 5);
            Debug.WriteLine(list.ToString());


            IEntity root = AtlasEntity.Instance;
            root.AddComponent<IEngine, AtlasEngine>(AtlasEngine.Instance);
            root.AddSystem<TestSystem>();

            Vector2 vector = new Vector2(-6, -5);
            vector.Reflect2(new Vector2(5, 5));
            //vector.ReflectAround2(new Vector2(5, 5), new Vector2(1, 1));

            for(int index1 = 1; index1 <= 5; ++index1)
            {
                string name1 = "0-" + index1 + "-0";
                IEntity child1 = root.AddChild(new AtlasEntity(name1, name1));
                child1.AddComponent<TestComponent>();
                child1.AddSystem<TestSystem>();
                for(int index2 = 1; index2 <= 5; ++index2)
                {
                    string name2 = "0-" + index1 + "-" + index2;
                    IEntity child2 = child1.AddChild(new AtlasEntity(name2, name2));
                    //child2.AddComponent<TestComponent>();
                }
            }

            root.AddChild();
            root.AddChild();
            Debug.WriteLine("== Names == ");
            foreach(IEntity child in root.Children)
            {
                Debug.WriteLine(child.GlobalName);
            }

            Debug.WriteLine("== One Removed == ");
            foreach(IEntity child in root.Children)
            {
                if(child.GlobalName == "0-2-0")
                {
                    child.Dispose();
                }
                else
                {
                    Debug.WriteLine(child.GlobalName);
                }

            }

            Debug.WriteLine(root.ToString());

            root.GetComponent<IEngine>().Run();
        }
    }
}