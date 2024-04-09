using System.Numerics;

namespace Atlas.Core.Objects.Update;

public interface IUpdate<T> where T : INumber<T>
{
	void Update(T time);
}