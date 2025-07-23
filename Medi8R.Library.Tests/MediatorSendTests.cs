using Medi8R.Library.Interfaces;
using Medi8R.Library.Services;
using Medi8R.Library.Tests.Stubs;

namespace Medi8R.Library.Tests
{
    [TestFixture]
    public class MediatorSendTests
    {
        [Test]
        public async Task Send_ReturnsExpectedValue()
        {
            StubRequest request = new("hello");

            IMediator mediator = new Mediator(type =>
            {
                if (type == typeof(IRequestHandler<StubRequest, string>))
                {
                    return new StubRequestHandler();
                }

                throw new InvalidOperationException($"Handler not found: {type}");
            });

            string result = await mediator.Send(request);

            Assert.That(result, Is.EqualTo("Handled: hello"));
        }

        [Test]
        public void Send_ThrowsIfHandlerNotRegistered()
        {
            StubRequest request = new("hello");

            IMediator mediator = new Mediator(type => null!);

            InvalidOperationException ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await mediator.Send(request));

            Assert.That(ex.Message, Does.Contain("Handler not found"));
        }

        [Test]
        public void Send_ThrowsIfMultipleHandlersExist()
        {
            StubRequest request = new("duplicate");

            IMediator mediator = new Mediator(type =>
            {
                if (type == typeof(IRequestHandler<StubRequest, string>))
                {
                    return new List<object>
                    {
                        new StubRequestHandler(),
                        new StubRequestHandler()
                    };
                }

                return null!;
            });

            InvalidOperationException ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                mediator.Send(request));

            Assert.That(ex.Message, Does.Contain("Multiple"));
        }

        [Test]
        public async Task Send_RequestWithoutResponse_ReturnsUnit()
        {
            DoSomethingCommand request = new();

            IMediator mediator = new Mediator(type =>
            {
                if (type == typeof(IRequestHandler<DoSomethingCommand, Unit>))
                {
                    return new DoSomethingCommandHandler();
                }

                throw new InvalidOperationException();
            });

            Unit result = await mediator.Send(request);

            Assert.That(result, Is.EqualTo(Unit.Value));
        }
    }
}