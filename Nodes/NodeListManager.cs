using Atlas.Components;
using Atlas.Entities;
using Atlas.Signals;
using Atlas.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.Nodes
{
    class NodeListManager : Component
    {
        private NodeList first;
	    private NodeList last;

        private Dictionary<Type, NodeList> nodeTypes = new Dictionary<Type, NodeList>();
        private List<NodeList> nodeListsPooled = new List<NodeList>();
        private List<NodeList> nodeListsRemoved = new List<NodeList>();
        private Signal<NodeListManager, Type> nodeListAdded = new Signal<NodeListManager, Type>();
	    private Signal<NodeListManager, Type> nodeListRemoved = new Signal<NodeListManager, Type>();
        
        public NodeListManager() : base(false)
        {

        }

        override protected void AddingComponentManager(Entity root)
	    {
		    base.AddingComponentManager(root);
		
		    root.ComponentAdded.Add(RootComponentAdded, int.MinValue + 1);
		    root.ComponentRemoved.Add(RootComponentRemoved, int.MinValue + 1);
		    if(root.HasComponent(typeof(EntityManager)))
		    {
			    RootComponentAdded(root, typeof(EntityManager));
		    }
		    if(root.HasComponent(typeof(SystemManager)))
		    {
			    RootComponentAdded(root, typeof(SystemManager));
		    }
	    }
	
	    override protected void RemovingComponentManager(Entity root)
	    {
		    root.ComponentAdded.Remove(RootComponentAdded);
		    root.ComponentRemoved.Remove(RootComponentRemoved);
		    if(root.HasComponent(typeof(EntityManager)))
		    {
			    RootComponentRemoved(root, typeof(EntityManager));
		    }
		    if(root.HasComponent(typeof(SystemManager)))
		    {
			    RootComponentRemoved(root, typeof(SystemManager));
		    }
		
		    base.RemovingComponentManager(root);
	    }

        private void RootComponentAdded(Entity root, Type componentType)
	    {
		    if(componentType == typeof(EntityManager))
		    {
			    EntityManager entityManager = root.GetComponent(typeof(EntityManager)) as EntityManager;
			    //Priority is SystemManager->NodeListManager->NodeList.
			    entityManager.EntityAdded.Add(EntityAdded, int.MinValue + 1);
			    entityManager.EntityRemoved.Add(EntityRemoved, int.MinValue + 1);
			    foreach(Entity entity in entityManager.Entities)
			    {
				    EntityAdded(entityManager, entity);
			    }
		    }
		    else if(componentType == typeof(SystemManager))
		    {
			    SystemManager systemManager = root.GetComponent(typeof(SystemManager)) as SystemManager;
			    systemManager.IsUpdatingChanged.Add(DisposeNodeLists, int.MinValue);
			    systemManager.SystemAdded.Add(SystemAdded, int.MinValue);
			    systemManager.SystemRemoved.Add(SystemRemoved, int.MinValue);
			    foreach(Type systemType in systemManager.SystemTypes)
			    {
				    SystemAdded(systemManager, systemType);
			    }
		    }
	    }
	
	    private void RootComponentRemoved(Entity root, Type componentType)
	    {
		    if(componentType == typeof(EntityManager))
		    {
			    EntityManager entityManager = root.GetComponent(typeof(EntityManager)) as EntityManager;
			    entityManager.EntityAdded.Remove(EntityAdded);
			    entityManager.EntityRemoved.Remove(EntityRemoved);
			    foreach(Entity entity in entityManager.Entities)
			    {
				    EntityRemoved(entityManager, entity);
			    }
		    }
		    else if(componentType == typeof(SystemManager))
		    {
			    SystemManager systemManager = root.GetComponent(typeof(SystemManager)) as SystemManager;
			    systemManager.IsUpdatingChanged.Remove(DisposeNodeLists);
			    systemManager.SystemAdded.Remove(SystemAdded);
			    systemManager.SystemRemoved.Remove(SystemRemoved);
			    foreach(Type systemType in systemManager.SystemTypes)
			    {
				    SystemRemoved(systemManager, systemType);
			    }
		    }
	    }

        private void EntityAdded(EntityManager entityManager, Entity entity)
	    {
		    //Priority is SystemManager->NodeListManager->NodeList.
		    entity.ComponentAdded.Add(ComponentAdded, int.MinValue + 1);
		    entity.ComponentRemoved.Add(ComponentRemoved, int.MinValue + 1);
            for(NodeList current = first; current != null; current = current.next)
            {
                current.EntityAdded(entity);
            }
	    }
	
	    private void EntityRemoved(EntityManager entityManager, Entity entity)
	    {
		    entity.ComponentAdded.Remove(ComponentAdded);
		    entity.ComponentRemoved.Remove(ComponentRemoved);
            for(NodeList current = first; current != null; current = current.next)
            {
                current.EntityRemoved(entity);
            }
        }

        private void ComponentAdded(Entity entity, Type componentType)
	    {
            for(NodeList current = first; current != null; current = current.next)
            {
                current.ComponentAdded(entity, componentType);
            }
        }
	
	    private void ComponentRemoved(Entity entity, Type componentType)
	    {
		    for(NodeList current = first; current != null; current = current.next)
            {
                current.ComponentRemoved(entity, componentType);
            }
        }

        private void SystemAdded(SystemManager systemManager, Type systemType)
	    {
		    AtlasSystem system = systemManager.GetSystemByType(systemType);
		    system.NodeTypeAdded.Add(NodeTypeAdded);
		    system.NodeTypeRemoved.Add(NodeTypeRemoved);
		    foreach(Type nodeType in system.NodeTypes)
		    {
			    NodeTypeAdded(system, nodeType);
		    }
	    }
	
	    private void SystemRemoved(SystemManager systemManager, Type systemType)
	    {
		    AtlasSystem system = systemManager.GetSystemByType(systemType);
		    system.NodeTypeAdded.Remove(NodeTypeAdded);
		    system.NodeTypeRemoved.Remove(NodeTypeRemoved);
		    foreach(Type nodeType in system.NodeTypes)
		    {
			    NodeTypeRemoved(system, nodeType);
		    }
	    }

        private void NodeTypeAdded(AtlasSystem system, Type nodeType)
	    {
		    if(!nodeTypes.ContainsKey(nodeType))
		    {
                NodeList nodeList;

			    if(nodeListsPooled.Count > 0)
			    {
                    nodeList = nodeListsPooled[nodeListsPooled.Count - 1];
                    nodeListsPooled.RemoveAt(nodeListsPooled.Count - 1);
			    }
			    else
			    {
				    nodeList = new NodeList();
			    }
			
			    nodeTypes.Add(nodeType, nodeList);

                nodeList.NodeListManager = this;
                nodeList.NodeType = nodeType;
			    
			    if(first == null)
			    {
				    first 			    = nodeList;
				    last 			    = nodeList;
				    nodeList.previous 	= null;
				    nodeList.next 		= null;
			    }
			    else
			    {
				    last.next 		    = nodeList;
				    nodeList.previous 	= last;
				    nodeList.next 		= null;
				    last 			    = nodeList;
			    }
			    
			    EntityManager entityManager = ComponentManager.GetComponent(typeof(EntityManager)) as EntityManager;
                foreach(Entity entity in entityManager.Entities)
                {
                    nodeList.EntityAdded(entity);
                }

                ++nodeList.totalReferences;

                nodeListAdded.Dispatch(this, nodeType);
		    }
		    else
		    {
                ++nodeTypes[nodeType].totalReferences;
		    }
	    }

        private void NodeTypeRemoved(AtlasSystem system, Type nodeType)
	    {
		    if(nodeTypes.ContainsKey(nodeType))
		    {
                NodeList nodeList = nodeTypes[nodeType];

                --nodeList.totalReferences;

                if(nodeList.totalReferences == 0)
			    {
				    nodeListRemoved.Dispatch(this, nodeType);
				
				    nodeTypes.Remove(nodeType);
				
				    if(nodeList == first)
				    {
					    first = first.next;
				    }
				    if(nodeList == last)
				    {
					    last = last.previous;
				    }
				    if(nodeList.previous != null)
				    {
					    nodeList.previous.next = nodeList.next;
				    }
				    if(nodeList.next != null)
				    {
					    nodeList.next.previous = nodeList.previous;
				    }
				
				    nodeListsRemoved.Add(nodeList);
			    }
		    }
	    }

        private void DisposeNodeLists(SystemManager systemManager, bool previousIsUpdating)
	    {
		    if(!systemManager.IsUpdating)
		    {
                for(NodeList current = first; current != null; current = current.next)
                {
                    current.DisposeNodes();
                }
			
			    while(nodeListsRemoved.Count > 0)
			    {
                    NodeList nodeList = nodeListsRemoved[nodeListsRemoved.Count - 1];
                    nodeListsRemoved.RemoveAt(nodeListsRemoved.Count - 1);
				    DisposeNodeList(nodeList);
			    }
		    }
	    }
	
	    private void DisposeNodeList(NodeList nodeList)
	    {
		    nodeList.NodeListManager = null;
		    nodeList.Dispose();
		    nodeListsPooled.Add(nodeList);
	    }

        
	    public int NumNodeLists
	    {
            get
            {
                int numNodeLists = 0;
                for(NodeList current = first; current != null; current = current.next)
                {
                    ++numNodeLists;
                }
                return numNodeLists;
            }
	    }
	
	    public List<NodeList> NodeLists
	    {
            get
            {
                List<NodeList> nodeLists = new List<NodeList>();
                for(NodeList current = first; current != null; current = current.next)
                {
                    nodeLists.Add(current);
                }
                return nodeLists;
            }
        }
	
	    public List<Type> NodeTypes
        {
            get
            {
                return new List<Type>(nodeTypes.Keys);
            }
        }
	
	    public Signal<NodeListManager, Type> NodeListAdded
	    {
            get
            {
                return nodeListAdded;
            }
	    }

        public Signal<NodeListManager, Type> NodeListRemoved
        {
            get
            {
                return nodeListRemoved;
            }
        }

        public NodeList GetNodeListByType(Type nodeType)
	    {
		    return nodeTypes.ContainsKey(nodeType) ? nodeTypes[nodeType] : null;
	    }
    }
}
