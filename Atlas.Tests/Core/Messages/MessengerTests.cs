using Atlas.Core.Messages;
using Atlas.Signals.Slots;
using Atlas.Tests.Core.Messages.Classes;
using NUnit.Framework;

namespace Atlas.Tests.Core.Messages;

[TestFixture]
class MessengerTests
{
	private TestMessenger Messenger;
	private bool Listened = false;

	[SetUp]
	public void SetUp()
	{
		Messenger = new TestMessenger();
		Listened = false;
	}

	#region Add
	[Test]
	public void When_AddListener_Then_ListenerAdded()
	{
		var slot = AddListener();

		Message();

		AssertSlot(slot, 0, true);
	}

	[Test]
	public void When_AddListener_Twice_Then_ListenerAdded()
	{
		var slot1 = AddListener();
		var slot2 = AddListener();

		Message();

		Assert.That(slot1 == slot2);
		AssertSlot(slot1, 0, true);
	}

	[TestCase(0)]
	[TestCase(1)]
	[TestCase(11)]
	[TestCase(-53)]
	[TestCase(5928)]
	[TestCase(int.MinValue)]
	[TestCase(int.MaxValue)]
	public void When_AddListener_WithPriority_Then_ListenerAdded(int priority)
	{
		var slot = AddListener(priority);

		Message();

		AssertSlot(slot, priority, true);
	}
	#endregion

	#region Remove
	[Test]
	public void When_RemoveListener_Then_ListenerRemoved()
	{
		AddListener();

		var success = RemoveListener();

		Message();

		Assert.That(success);
		AssertSlot(null, null, false);
	}

	[Test]
	public void When_RemoveListener_Then_NoListenerRemoved()
	{
		var success = RemoveListener();

		Message();

		Assert.That(!success);
		AssertSlot(null, null, false);
	}

	[Test]
	public void When_RemoveListeners_Then_ListenersRemoved()
	{
		AddListener();

		var success = Messenger.RemoveListeners();

		Message();

		Assert.That(success);
		Assert.That(Messenger.GetListener<IMessage<TestMessenger>>(Listen) == null);
		Assert.That(!HasListener());
		Assert.That(!Listened);
	}
	#endregion

	[Test]
	public void When_Message_Then_MessengersEqual()
	{
		var message = new Message<TestMessenger>();
		Messenger.Message<IMessage<TestMessenger>>(message);
		Assert.That((message as IMessage).Messenger == Messenger);
		Assert.That((message as IMessage).CurrentMessenger == Messenger);
	}

	#region Helpers
	private void AssertSlot(ISlot<IMessage<TestMessenger>> other, int? priority, bool expected)
	{
		var slot = GetListener();

		Assert.That(slot == other);
		Assert.That(slot?.Listener == Listen == expected);
		Assert.That(slot?.Priority == priority);
		Assert.That(HasListener() == expected);
		Assert.That(Listened == expected);
	}

	private ISlot<IMessage<TestMessenger>> AddListener(int priority = 0) => Messenger.AddListener<IMessage<TestMessenger>>(Listen, priority);

	private bool RemoveListener() => Messenger.RemoveListener<IMessage<TestMessenger>>(Listen);

	private ISlot<IMessage<TestMessenger>> GetListener() => Messenger.GetListener<IMessage<TestMessenger>>(Listen);

	private bool HasListener() => Messenger.HasListener<IMessage<TestMessenger>>(Listen);

	private void Message() => Messenger.Message<IMessage<TestMessenger>>(new Message<TestMessenger>());

	private void Listen(IMessage<TestMessenger> message)
	{
		Listened = true;
	}
	#endregion
}