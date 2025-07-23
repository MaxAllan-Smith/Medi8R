using Medi8R.Library.Interfaces;

namespace Medi8R.Library.Tests.Stubs;

public class StubRequest(string input) : IRequest<string>
{
    public string Input { get; } = input;
}