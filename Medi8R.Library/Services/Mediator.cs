using Medi8R.Library.Interfaces;
using System.Reflection;

namespace Medi8R.Library.Services
{
    public class Mediator(Func<Type, object> serviceFactory) : IMediator
    {
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Type handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(request.GetType(), typeof(TResponse));

            object? handler = serviceFactory(handlerType);
            if (handler is null)
            {
                throw new InvalidOperationException($"Handler not found for type {handlerType.FullName}");
            }

            MethodInfo? handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod is null)
            {
                throw new InvalidOperationException("Handler does not implement Handle method");
            }

            object? result = handleMethod.Invoke(handler, new[] { request });
            if (result is not Task<TResponse> task)
            {
                throw new InvalidOperationException("Handler returned unexpected result type");
            }

            return task;

        }
    }
}