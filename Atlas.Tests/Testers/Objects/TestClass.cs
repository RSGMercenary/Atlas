using System;

namespace Atlas.Tests.Testers.Objects;

internal class TestClass : IDisposable
{
	public bool TestDispose = false;

	public TestClass() { }

	public void Dispose()
	{
		TestDispose = true;
	}
}