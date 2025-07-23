using Medi8R.Library.Interfaces;

namespace Medi8R.Library.Tests.Stubs;

public class StubRequestHandler : IRequestHandler<StubRequest, string>
{
    public Task<string> Handle(StubRequest request)
    {
        return Task.FromResult($"Handled: {request.Input}");
    }
}