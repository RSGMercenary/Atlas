using System;

namespace Atlas.Core.Objects.Update;

public interface IUpdateRunner
{
	event Action<IUpdateRunner, bool> IsRunningChanged;

	bool IsRunning { get; set; }
}