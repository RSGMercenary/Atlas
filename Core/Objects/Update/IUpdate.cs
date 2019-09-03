using System;

namespace Atlas.Core.Objects.Update
{
	public interface IUpdate<T>
		where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		void Update(T time);
	}
}
