using Atlas.Core.Messages;

namespace Atlas.ECS.Families
{
	#region Interfaces

	public interface IFamilyMemberAddMessage<TFamilyMember> : IValueMessage<IFamily<TFamilyMember>, TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{ }

	public interface IFamilyMemberRemoveMessage<TFamilyMember> : IValueMessage<IFamily<TFamilyMember>, TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{ }

	#endregion

	#region Classes

	class FamilyMemberAddMessage<TFamilyMember> : ValueMessage<IFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberAddMessage<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		public FamilyMemberAddMessage(TFamilyMember value) : base(value) { }
	}

	class FamilyMemberRemoveMessage<TFamilyMember> : ValueMessage<IFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberRemoveMessage<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		public FamilyMemberRemoveMessage(TFamilyMember value) : base(value) { }
	}

	#endregion
}