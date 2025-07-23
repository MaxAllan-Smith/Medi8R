using Medi8R.Library.Interfaces;
using Medi8R.Library.Services;

namespace Medi8R.Library.Tests
{
    public class TestNotification : INotification { }

    public class FirstHandler : INotificationHandler<TestNotification>
    {
        public bool WasCalled { get; private set; }

        public Task Handle(TestNotification notification)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }

    public class SecondHandler : INotificationHandler<TestNotification>
    {
        public bool WasCalled { get; private set; }

        public Task Handle(TestNotification notification)
        {
            WasCalled = true;
            return Task.CompletedTask;
        }
    }

    [TestFixture]
    public class MediatorPublishTests
    {
        [Test]
        public async Task Publish_CallsAllHandlers()
        {
            TestNotification notification = new();

            FirstHandler first = new();
            SecondHandler second = new();

            IMediator mediator = new Mediator(type => type == typeof(INotificationHandler<TestNotification>) ? new List<object> { first, second } : null!);

            await mediator.Publish(notification);

            Assert.That(first.WasCalled, Is.True);
            Assert.That(second.WasCalled, Is.True);
        }
    }
}
