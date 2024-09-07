using Atlas.ECS.Families;


namespace Atlas.ECS.Components.Engine.Families;
public interface IFamilyConstructor
{
	IFamily<TFamilyMember> Construct<TFamilyMember>() where TFamilyMember : class, IFamilyMember, new();
}