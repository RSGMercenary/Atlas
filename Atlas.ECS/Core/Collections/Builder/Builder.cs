namespace Atlas.Core.Collections.Builder;

public abstract class Builder<TBuilder, T> : IBuilder<TBuilder, T>
		where TBuilder : class, IBuilder<TBuilder, T>
		where T : class
{
	protected T Instance { get; private set; }

	protected Builder() { Start(); }
	protected Builder(T instance) { Instance = instance; }

	public TBuilder Start()
	{
		Instance ??= NewInstance();
		return this as TBuilder;
	}

	protected abstract T NewInstance();

	public T Finish()
	{
		var instance = Instance;
		Instance = null;
		return instance;
	}
}