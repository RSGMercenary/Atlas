using Atlas.Engine.Interfaces;

namespace Atlas.Engine
{
	interface IBaseObject<T>:IDispose<T>, IAutoDispose
	{
	}
}
