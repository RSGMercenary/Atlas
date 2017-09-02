namespace Atlas.Engine.Messages
{
	static class AtlasMessage
	{
		//EngineObject
		public const string Engine = "Engine";
		public const string State = "State";

		//AutoEngineObject
		public const string AutoDestroy = "AutoDestroy";

		//Engine
		public const string AddEntity = "AddEntity";
		public const string RemoveEntity = "RemoveEntity";
		public const string AddSystem = "AddSystem";
		public const string RemoveSystem = "RemoveSystem";
		public const string AddFamily = "AddFamily";
		public const string RemoveFamily = "RemoveFamily";
		public const string Update = "Update";

		//Entity
		public const string GlobalName = "GlobalName";
		public const string LocalName = "LocalName";
		public const string Root = "Root";
		public const string Parent = "Parent";
		public const string ParentIndex = "ParentIndex";
		public const string AddChild = "AddChild";
		public const string RemoveChild = "RemoveChild";
		public const string Children = "Children";
		public const string AddComponent = "AddComponent";
		public const string RemoveComponent = "RemoveComponent";
		public const string AddSystemType = "AddSystemType";
		public const string RemoveSystemType = "RemoveSystemType";
		public const string Sleeping = "Sleeping";
		public const string FreeSleeping = "FreeSleeping";

		//Component
		public const string AddManager = "AddManager";
		public const string RemoveManager = "RemoveManager";
		public const string Managers = "Managers";

		//System
		public const string Priority = "Priority";
		public const string UpdatePhase = "IsUpdating";
	}
}
