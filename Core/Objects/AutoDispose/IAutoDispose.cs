using System;

namespace Atlas.Core.Objects.AutoDispose
{
	public interface IAutoDispose : IDisposable
	{
		bool AutoDispose { get; set; }
	}
}