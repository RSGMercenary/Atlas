using Atlas.Core.Collections.Hierarchy;

namespace Atlas.Core.Objects.Builder
{
	public interface IHierarchyBuilder<TBuilder, T> : IMessengerBuilder<TBuilder, T>
		where TBuilder : IHierarchyBuilder<TBuilder, T>
		where T : class, IHierarchy<T>
	{
		TBuilder SetRoot(bool root);

		TBuilder AddChild(T child);

		TBuilder AddChild(T child, int index);

		TBuilder RemoveChild(T child);

		TBuilder RemoveChild(int index);

		TBuilder SetParent(T parent);

		TBuilder SetParent(T parent, int index);
	}
}