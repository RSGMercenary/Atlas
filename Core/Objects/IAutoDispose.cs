using System;

namespace Atlas.Core.Objects
{
	public interface IAutoDispose : IDisposable
	{
		bool AutoDispose { get; set; }
	}
}