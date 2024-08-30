using Atlas.Core.Objects.Update;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Systems;

[TestFixture]
class UpdateRunnerTests
{
	[TestCase(true)]
	[TestCase(false)]
	public void When_IsRunning_And_Stopped_Then_IsRunningFalse(bool isRunning)
	{
		var system = new TestSystem();
		var updater = new UpdateRunner<float>(system);

		system.TestAction = () => updater.IsRunning = false;
		updater.IsRunning = isRunning;

		Assert.That(!updater.IsRunning);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_Update_Then_ThrowsExpected(bool instance)
	{
		var system = instance ? new TestSystem() : null;

		Assert.That(() => new UpdateRunner<float>(system), instance ? Throws.Nothing : Throws.Exception);
	}
}