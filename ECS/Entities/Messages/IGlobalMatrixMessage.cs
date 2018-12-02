using Atlas.Core.Messages;
using Microsoft.Xna.Framework;

namespace Atlas.ECS.Entities.Messages
{
	public interface IGlobalMatrixMessage : IPropertyMessage<IEntity, Matrix>
	{
		/// <summary>
		/// Determines whether the Entity's GlobalMatrix was altered due to a LocalMatrix change
		/// or the recalculation of a Parent/Ancestor GlobalMatrix.
		/// </summary>
		bool Hierarchy { get; }
	}
}
