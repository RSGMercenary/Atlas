using System;

namespace Atlas.Tests.Core.Collections.Classes;

internal class TestInstance : IDisposable
{
	public bool TestDispose = false;

	public TestInstance() { }

	public void Dispose()
	{
		TestDispose = true;
	}
}