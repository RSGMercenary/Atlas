using System.Collections.Generic;

namespace Atlas.Testing.CAT
{
	class ConditionList : Condition
	{
		private List<ICondition> conditions = new List<ICondition>();
		private ConditionType type = ConditionType.And;
		private ActionList actions = new ActionList();

		public ICondition AddCondition(ICondition condition)
		{
			if(condition == null)
				return null;
			if(conditions.Contains(condition))
				return null;
			conditions.Add(condition);
			//Check();
			return condition;
		}

		override protected void Check()
		{
			foreach(var condition in conditions)
			{
				if(condition.IsTrue)
				{
					if(type == ConditionType.Or)
					{
						IsTrue = true;
					}
				}
				else
				{
					if(type == ConditionType.And)
					{
						IsTrue = false;
					}
				}
			}
			if(type == ConditionType.And)
			{
				IsTrue = true;
			}
			else
			{
				IsTrue = false;
			}
		}
	}
}
