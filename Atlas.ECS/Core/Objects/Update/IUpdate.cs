using System.Numerics;

namespace Atlas.Core.Objects.Update;

/// <summary>
/// An <see langword="interface"/> with an <see cref="Update"/> <see langword="method"/> for providing an update loop.
/// </summary>
/// <typeparam name="T">The <see cref="INumber{TSelf}"/> precision of the update loop.</typeparam>
public interface IUpdate<T> where T : INumber<T>
{
	/// <summary>
	/// The <see langword="method"/> for prodiving an update loop.
	/// </summary>
	/// <param name="deltaTime">The elapsed time for the update.</param>
	void Update(T deltaTime);
}