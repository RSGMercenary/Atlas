using Atlas.Entities;
using Atlas.Signals;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.Nodes
{
	sealed class NodeList
	{
		internal NodeList previous;
		internal NodeList next;
		internal int totalReferences = 0;

		private NodeListManager nodeListManager;
		private Signal<NodeList, NodeListManager> nodeListManagerChanged = new Signal<NodeList, NodeListManager>();

		private List<Node> nodesPooled = new List<Node>();
		private List<Node> nodesRemoved = new List<Node>();

		private Type nodeType;
		private Dictionary<Type, string> components = new Dictionary<Type, string>();
		private Dictionary<Entity, Node> entities = new Dictionary<Entity, Node>();

		private Node first;
		private Node last;

		private Signal<NodeList, Node> nodeAdded = new Signal<NodeList, Node>();
		private Signal<NodeList, Node> nodeRemoved = new Signal<NodeList, Node>();

		internal NodeList()
		{

		}

		internal void Dispose()
		{
			NodeListManager = null;
			NodeType = null;
		}

		public NodeListManager NodeListManager
		{
			get
			{
				return nodeListManager;
			}
			internal set
			{
				if(nodeListManager != value)
				{
					NodeListManager previous = nodeListManager;
					nodeListManager = value;
					nodeListManagerChanged.Dispatch(this, previous);
				}
			}
		}

		public Type NodeType
		{
			get
			{
				return nodeType;
			}
			internal set
			{
				if(nodeType != value)
				{
					nodeType = value;
					foreach(Type componentType in components.Keys)
					{
						components.Remove(componentType);
					}
					if(nodeType != null)
					{
						//TO-DO :: This only gets public fields of a Node Type.
						//If Components on a Node should have setters/getters, then we'll want private fields.
						FieldInfo[] fields = nodeType.GetFields();
						foreach(FieldInfo field in fields)
						{
							components.Add(field.FieldType, field.Name);
						}
					}
				}
			}
		}

		public Signal<NodeList, Node> NodeAdded
		{
			get
			{
				return nodeAdded;
			}
		}

		public Signal<NodeList, Node> NodeRemoved
		{
			get
			{
				return nodeRemoved;
			}
		}

		internal void EntityAdded(Entity entity)
		{
			AddNode(entity);
		}

		internal void EntityRemoved(Entity entity)
		{
			RemoveNode(entity);
		}

		internal void ComponentAdded(Entity entity, Type componentType)
		{
			if(components.ContainsKey(componentType))
			{
				AddNode(entity);
			}
		}

		internal void ComponentRemoved(Entity entity, Type componentType)
		{
			if(components.ContainsKey(componentType))
			{
				RemoveNode(entity);
			}
		}

		private void AddNode(Entity entity)
		{
			if(!entities.ContainsKey(entity))
			{
				if(components.Count > 0)
				{
					foreach(Type componentType in components.Keys)
					{
						if(!entity.HasComponent(componentType))
						{
							return;
						}
					}
				}

				Node node;

				if(nodesPooled.Count > 0)
				{
					node = nodesPooled[nodesPooled.Count - 1];
					nodesPooled.RemoveAt(nodesPooled.Count - 1);
				}
				else
				{
					node = Activator.CreateInstance(nodeType) as Node;
				}

				entities.Add(entity, node);

				//node.NodeList = this;
				node.Entity = entity;

				if(first == null)
				{
					first = node;
					last = node;
					node.Previous = null;
					node.Next = null;
				}
				else
				{
					last.Next = node;
					node.Previous = last;
					node.Next = null;
					last = node;
				}

				if(components.Count > 0)
				{
					foreach(Type componentType in components.Keys)
					{
						FieldInfo field = componentType.GetField(components[componentType]);
						field.SetValue(node, entity.GetComponent(componentType));
					}
				}

				nodeAdded.Dispatch(this, node);
			}
		}

		private void RemoveNode(Entity entity)
		{
			if(entities.ContainsKey(entity))
			{
				Node node = entities[entity];

				nodeRemoved.Dispatch(this, node);

				entities.Remove(entity);

				if(node == first)
					first = first.Next;
				if(node == last)
					last = last.Previous;
				if(node.Previous != null)
					node.Previous.Next = node.Next;
				if(node.Next != null)
					node.Next.Previous = node.Previous;

				nodesRemoved.Add(node);
			}
		}

		internal void DisposeNodes()
		{
			while(nodesRemoved.Count > 0)
			{
				Node node = nodesRemoved[nodesRemoved.Count - 1];
				nodesRemoved.RemoveAt(nodesRemoved.Count - 1);
				DisposeNode(node);
			}
		}

		private void DisposeNode(Node node)
		{
			foreach(Type componentType in components.Keys)
			{
				FieldInfo field = componentType.GetField(components[componentType]);
				field.SetValue(node, null);
			}

			//node.NodeList = null;
			node.Entity = null;
			node.Previous = null;
			node.Next = null;

			nodesPooled.Add(node);
		}
	}
}
