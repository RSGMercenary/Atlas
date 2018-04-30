using Atlas.Engine.Entities;
using Atlas.Engine.Families;

namespace Atlas.Engine.Messages
{
	class FamilyEntityAddMessage : ValueMessage<IFamily, IEntity>, IFamilyEntityAddMessage
	{
		public FamilyEntityAddMessage(IEntity value) : base(value)
		{
		}
	}
}
