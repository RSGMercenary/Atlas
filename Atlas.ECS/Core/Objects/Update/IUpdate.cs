using System.Numerics;

namespace Atlas.Core.Objects.Update;

public interface IUpdate<T> : IUpdateState where T : INumber<T>
{
	void Update(T time);
}