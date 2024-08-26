namespace Atlas.Core.Objects.Builder;

public interface IBuilder<out TBuilder, out T>
		where TBuilder : IBuilder<TBuilder, T>
		where T : class
{
	TBuilder Start();
	T Finish();
}