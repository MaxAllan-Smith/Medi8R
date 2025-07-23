using Medi8R.Library.Interfaces;

namespace Medi8R.Library.Tests.Stubs
{
    public class DoSomethingCommandHandler : IRequestHandler<DoSomethingCommand, Unit>
    {
        public Task<Unit> Handle(DoSomethingCommand request)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}