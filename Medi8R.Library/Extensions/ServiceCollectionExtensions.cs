using Medi8R.Library.Interfaces;
using Medi8R.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Medi8R.Library.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMedi8R(this IServiceCollection services, Assembly assembly)
        {
            services.AddSingleton<IMediator, Mediator>(sp =>
                new Mediator(sp.GetRequiredService));

            IEnumerable<Type> types = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface);

            foreach (Type type in types)
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType)
                    {
                        continue;
                    }

                    Type genericDef = iface.GetGenericTypeDefinition();

                    if (genericDef == typeof(IRequestHandler<,>) ||
                        genericDef == typeof(INotificationHandler<>))
                    {
                        services.AddTransient(iface, type);
                    }
                }
            }

            return services;
        }
    }
}