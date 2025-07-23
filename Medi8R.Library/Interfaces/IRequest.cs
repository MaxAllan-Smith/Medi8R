namespace Medi8R.Library.Interfaces
{
    public interface IRequest<TResponse> { }

    public interface IRequest : IRequest<Unit> { }
}