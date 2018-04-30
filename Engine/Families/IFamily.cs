using Atlas.Engine.Collections.EngineList;

namespace Atlas.Engine.Families
{
	public interface IFamily<TFamilyMember> : IFamily
		where TFamilyMember : IFamilyMember, new()
	{
		new IReadOnlyEngineList<TFamilyMember> Members { get; }
	}
}