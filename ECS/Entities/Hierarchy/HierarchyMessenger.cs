using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Entities
{
	public class HierarchyMessenger
	{
		public static Func<IMessage<IEntity>, bool> From(Hierarchy hierarchy)
		{
			return message =>
			{
				var first = message.Messenger as IEntity;
				var current = message.CurrentMessenger as IEntity;
				if(first == null || current == null)
					return false;
				if(hierarchy.HasFlag(Hierarchy.All))
					return true;
				if(hierarchy.HasFlag(Hierarchy.Self) && current == first)
					return true;
				if(hierarchy.HasFlag(Hierarchy.Parent) && current.Parent == first)
					return true;
				if(hierarchy.HasFlag(Hierarchy.Child) && current == first.Parent)
					return true;
				if(hierarchy.HasFlag(Hierarchy.Sibling) && current.HasSibling(first))
					return true;
				if(hierarchy.HasFlag(Hierarchy.Ancestor) && current.HasAncestor(first))
					return true;
				if(hierarchy.HasFlag(Hierarchy.Descendent) && current.HasDescendant(first))
					return true;
				return false;
			};
		}
	}
}
