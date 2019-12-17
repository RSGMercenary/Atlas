using System.Text;

namespace Atlas.ECS.Components.Component
{
	public static class ComponentExtensions
	{
		public static string ToInfoString(this IComponent component, bool addEntities, int index = 0, string indent = "", StringBuilder text = null)
		{
			text ??= new StringBuilder();
			text.Append($"{indent}Component");
			if(index > 0)
				text.Append($" {index}");
			text.AppendLine();
			text.AppendLine($"{indent}  Instance    = {component.GetType().FullName}");
			if(component.Manager != null)
				text.AppendLine($"{indent}  Interface   = {component.Manager.GetComponentType(component).FullName}");
			text.AppendLine($"{indent}  {nameof(component.AutoDispose)} = {component.AutoDispose}");
			text.AppendLine($"{indent}  {nameof(component.IsShareable)} = {component.IsShareable}");
			if(component.IsShareable)
			{
				text.AppendLine($"{indent}  Entities ({component.Managers.Count})");
				if(addEntities)
				{
					index = 0;
					foreach(var entity in component.Managers)
					{
						text.AppendLine($"{indent}    Entity {++index}");
						text.AppendLine($"{indent}      Interface  = {entity.GetComponentType(component).FullName}");
						text.AppendLine($"{indent}      {nameof(entity.GlobalName)} = {entity.GlobalName}");
					}
				}
			}
			return text.ToString();
		}
	}
}