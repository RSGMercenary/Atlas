using Atlas.Nodes;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Systems
{
	abstract class AtlasSystem
	{
		private SystemManager systemManager;
		private Signal<AtlasSystem, SystemManager> systemManagerChanged = new Signal<AtlasSystem, SystemManager>();

		internal int totalReferences = 0;

		private Type systemType;

		//private var _nodeAddedHandlers:ObjectMap<Dynamic, Dynamic> = new ObjectMap<Dynamic, Dynamic>();
		//private var _nodeRemovedHandlers:ObjectMap<Dynamic, Dynamic> = new ObjectMap<Dynamic, Dynamic>();

		private int totalSleeping = 0;
		private Signal<AtlasSystem, int> totalSleepingChanged = new Signal<AtlasSystem, int>();

		private int priority = 0;
		private Signal<AtlasSystem, int> priorityChanged = new Signal<AtlasSystem, int>();

		private bool isUpdating = false;
		private Signal<AtlasSystem, bool> isUpdatingChanged = new Signal<AtlasSystem, bool>();

		private HashSet<Type> nodeTypes = new HashSet<Type>();
		private Dictionary<Type, NodeList> nodeLists = new Dictionary<Type, NodeList>();
		private Signal<AtlasSystem, Type> nodeTypeAdded = new Signal<AtlasSystem, Type>();
		private Signal<AtlasSystem, Type> nodeTypeRemoved = new Signal<AtlasSystem, Type>();

		private Signal<AtlasSystem, bool> disposed = new Signal<AtlasSystem, bool>();

		//public var numSystemClassManagers(get, never):UInt;

		public AtlasSystem()
		{
			systemType = GetType();
		}

		internal void Dispose()
		{
			if(systemManager == null)
			{
				disposed.Dispatch(this, true);
				Disposing();
				SystemManager = null;
				systemManagerChanged.Dispose();
				isUpdatingChanged.Dispose();
				priorityChanged.Dispose();
				totalSleepingChanged.Dispose();
			}
		}

		protected virtual void Disposing()
		{

		}

		public Signal<AtlasSystem, bool> Disposed
		{
			get
			{
				return disposed;
			}
		}

		public SystemManager SystemManager
		{
			get
			{
				return systemManager;
			}
			internal set
			{
				if(systemManager != value)
				{
					SystemManager previous = systemManager;
					systemManager = value;
					systemManagerChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<AtlasSystem, SystemManager> SystemManagerChanged
		{
			get
			{
				return systemManagerChanged;
			}
		}

		internal void Update()
		{
			IsUpdating = true;
			Updating();
			IsUpdating = false;
		}

		protected virtual void Updating()
		{

		}

		public bool IsUpdating
		{
			get
			{
				return isUpdating;
			}
			internal set
			{
				if(isUpdating != value)
				{
					bool previous = isUpdating;
					isUpdating = value;
					isUpdatingChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<AtlasSystem, bool> IsUpdatingChanged
		{
			get
			{
				return isUpdatingChanged;
			}
		}

		public int TotalSleeping
		{
			get
			{
				return totalSleeping;
			}
			set
			{
				if(totalSleeping != value)
				{
					int previous = totalSleeping;
					totalSleeping = value;
					totalSleepingChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<AtlasSystem, int> TotalSleepingChanged
		{
			get
			{
				return totalSleepingChanged;
			}
		}

		public bool IsSleeping
		{
			get
			{
				return totalSleeping > 0;
			}
		}

		public int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				if(priority != value)
				{
					int previous = priority;
					priority = value;
					priorityChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<AtlasSystem, int> PriorityChanged
		{
			get
			{
				return priorityChanged;
			}
		}

		public Type SystemType
		{
			get
			{
				return systemType;
			}
		}

		public Signal<AtlasSystem, Type> NodeTypeAdded
		{
			get
			{
				return nodeTypeAdded;
			}
		}

		public Signal<AtlasSystem, Type> NodeTypeRemoved
		{
			get
			{
				return nodeTypeRemoved;
			}
		}

		public List<Type> NodeTypes
		{
			get
			{
				return new List<Type>(nodeTypes);
			}
		}

		public bool HasNodeType(Type type)
		{
			return nodeTypes.Contains(type);
		}

		protected bool AddNodeType(Type type)
		{
			if(type != null)
			{
				if(!nodeTypes.Contains(type))
				{
					nodeTypes.Add(type);
					nodeTypeAdded.Dispatch(this, type);
					return true;
				}
			}
			return false;
		}

		protected bool RemoveNodeType(Type type)
		{
			if(type != null)
			{
				if(nodeTypes.Contains(type))
				{
					nodeTypeRemoved.Dispatch(this, type);
					nodeTypes.Remove(type);
					return true;
				}
			}
			return false;
		}

		protected void RemoveNodeTypes()
		{
			/*
			foreach(Type type in nodeTypes.)
			{
				RemoveNodeType(type);
			}
			*/
		}

		public NodeList GetNodeList(Type nodeType)
		{
			return nodeLists[nodeType];
		}

		public List<NodeList> NodeLists
		{
			get
			{
				return new List<NodeList>(nodeLists.Values);
			}
		}

		internal void AddNodeList(NodeList nodeList)
		{
			nodeLists.Add(nodeList.NodeType, nodeList);
			AddingNodeList(nodeList);
		}

		protected virtual void AddingNodeList(NodeList nodeList)
		{
			/*
			var nodeRemoved:Dynamic = this._nodeRemovedHandlers.get(nodeList.nodeClass);
			if (nodeRemoved != null)

			{
				nodeList.nodeRemoved.add(nodeRemoved);
			}

			var nodeAdded:Dynamic = this._nodeAddedHandlers.get(nodeList.nodeClass);
			if (nodeAdded != null)
			{
				nodeList.nodeAdded.add(nodeAdded);
				var node:Node = nodeList.first;
				while (node != null)
				{
					nodeAdded(nodeList, node);
					node = node.next;
				}
			}
			*/
		}

		internal void RemoveNodeList(NodeList nodeList)
		{
			RemovingNodeList(nodeList);
			nodeLists.Remove(nodeList.NodeType);
		}

		protected virtual void RemovingNodeList(NodeList nodeList)
		{
			/*
			var nodeAdded:Dynamic = this._nodeAddedHandlers.get(nodeList.nodeClass);
			if (nodeAdded != null)

			{
				nodeList.nodeAdded.remove(nodeAdded);
			}

			var nodeRemoved:Dynamic = this._nodeRemovedHandlers.get(nodeList.nodeClass);
			if (nodeRemoved != null)
			{
				nodeList.nodeRemoved.remove(nodeRemoved);
				var node:Node = nodeList.first;
				while (node != null)
				{
					nodeRemoved(nodeList, node);
					node = node.next;
				}
			}
			*/
		}
	}
}
