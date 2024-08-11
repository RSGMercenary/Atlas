using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Atlas.Tests.ECS.Systems;

[TestFixture]
public class UpdaterTests
{
	[TestCase(true)]
	[TestCase(false)]
	public void When_IsRunning_And_Stopped_Then_IsRunningFalse(bool isRunning)
	{
		var system = new TestSystem();
		var updater = new Updater<float>(system);

		system.AddListener<IUpdateStateMessage<ISystem>>((message) => updater.IsRunning = isRunning);

		Assert.That(!updater.IsRunning);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_Update_Then_ThrowsExpected(bool instance)
	{
		var system = instance ? new TestSystem() : null;
		IResolveConstraint expected = instance ? Throws.Nothing : Throws.Exception;

		Assert.That(() => new Updater<float>(system), expected);
	}
}