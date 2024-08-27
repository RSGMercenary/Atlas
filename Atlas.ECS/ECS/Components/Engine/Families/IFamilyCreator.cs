using Atlas.ECS.Families;


namespace Atlas.ECS.Components.Engine.Families;
public interface IFamilyCreator
{
	IFamily<TFamilyMember> Create<TFamilyMember>() where TFamilyMember : class, IFamilyMember, new();
}