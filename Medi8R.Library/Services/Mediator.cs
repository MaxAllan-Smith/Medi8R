using System.Reflection;
using Medi8R.Library.Interfaces;

namespace Medi8R.Library.Services
{
    public class Mediator(Func<Type, object> serviceFactory) : IMediator
    {
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            Type handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(request.GetType(), typeof(TResponse));

            object handler = serviceFactory(handlerType);

            MethodInfo handleMethod = handlerType.GetMethod("Handle")
                                      ?? throw new InvalidOperationException("Handler does not implement Handle method");

            object? result = handleMethod.Invoke(handler, new[] { request });

            return (Task<TResponse>)result!;
        }
    }
}