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
            // Arrange
            StubRequest request = new("hello");

            // Manually wire handler
            IMediator mediator = new Mediator(type =>
            {
                if (type == typeof(IRequestHandler<StubRequest, string>))
                {
                    return new StubRequestHandler();
                }

                throw new InvalidOperationException($"Handler not found: {type}");
            });

            // Act
            string result = await mediator.Send(request);

            // Assert
            Assert.That(result, Is.EqualTo("Handled: hello"));
        }

        [Test]
        public void Send_ThrowsIfHandlerNotRegistered()
        {
            // Arrange
            StubRequest request = new("hello");

            IMediator mediator = new Mediator(type => null!); // simulate missing handler

            // Act & Assert
            InvalidOperationException ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await mediator.Send(request));

            Assert.That(ex.Message, Does.Contain("Handler not found"));
        }
    }
}
