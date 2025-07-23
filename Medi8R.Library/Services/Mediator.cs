using Medi8R.Library.Interfaces;
using System.Reflection;

namespace Medi8R.Library.Services
{
    public delegate object? ServiceFactory(Type serviceType);

    public class Mediator(ServiceFactory serviceFactory) : IMediator
    {
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Type handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(request.GetType(), typeof(TResponse));

            object? resolved = serviceFactory(handlerType);
            if (resolved is null)
            {
                throw new InvalidOperationException($"Handler not found for type {handlerType.FullName}");
            }

            object? handler = resolved;

            // If serviceFactory returns a collection, unwrap and validate
            if (resolved is IEnumerable<object> collection)
            {
                object[] items = collection.ToArray();
                if (items.Length > 1)
                {
                    throw new InvalidOperationException($"Multiple handlers found for {request.GetType().Name}");
                }

                handler = items.SingleOrDefault();
            }

            MethodInfo? handleMethod = handler?.GetType().GetMethod("Handle");
            if (handleMethod is null)
            {
                throw new InvalidOperationException("Handler does not implement Handle method");
            }

            object? result = handleMethod.Invoke(handler, [request]);
            if (result is not Task<TResponse> task)
            {
                throw new InvalidOperationException("Handler returned unexpected result type");
            }

            return task;
        }
    }
}