using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Entities
{
	public class HierarchyMessenger
	{
		public static Func<IMessage<IEntity>, bool> From(MessageFlow hierarchy)
		{
			return message =>
			{
				var first = message.Messenger as IEntity;
				var current = message.CurrentMessenger as IEntity;
				if(first == null || current == null)
					return false;
				if(hierarchy.HasFlag(MessageFlow.All))
					return true;
				if(hierarchy.HasFlag(MessageFlow.Self) && current == first)
					return true;
				if(hierarchy.HasFlag(MessageFlow.Parent) && current.Parent == first)
					return true;
				if(hierarchy.HasFlag(MessageFlow.Child) && current == first.Parent)
					return true;
				if(hierarchy.HasFlag(MessageFlow.Sibling) && current.HasSibling(first))
					return true;
				if(hierarchy.HasFlag(MessageFlow.Ancestor) && current.HasAncestor(first))
					return true;
				if(hierarchy.HasFlag(MessageFlow.Descendent) && current.HasDescendant(first))
					return true;
				return false;
			};
		}
	}
}
