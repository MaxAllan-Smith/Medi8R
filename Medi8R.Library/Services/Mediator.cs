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

            object? result = handleMethod.Invoke(handler, new[] { request });
            if (result is not Task<TResponse> task)
            {
                throw new InvalidOperationException("Handler returned unexpected result type");
            }

            return task;
        }

        public Task<Unit> Send(IRequest request)
        {
            return Send<Unit>(request);
        }

        public async Task Publish(INotification notification)
        {
            if (notification is null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            Type handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());

            object? resolved = serviceFactory(handlerType);
            if (resolved is null)
            {
                return;
            }

            IEnumerable<object> handlers = resolved switch
            {
                IEnumerable<object> collection => collection,
                not null => [resolved],
                _ => throw new InvalidOperationException("Invalid handler resolution result")
            };

            foreach (object handler in handlers)
            {
                MethodInfo? handleMethod = handler.GetType().GetMethod("Handle");

                if (handleMethod?.Invoke(handler, [notification]) is Task task)
                {
                    await task;
                }
            }
        }

    }
}