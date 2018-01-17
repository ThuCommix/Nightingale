using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nightingale.Sessions;

namespace Nightingale.Web
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds Nightingale as a service.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="connectionFactory">The connection factory.</param>
        public static void AddNightingale(this IServiceCollection serviceCollection, IConnectionFactory connectionFactory)
        {
            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.TryAddSingleton(connectionFactory);
            serviceCollection.TryAddSingleton<ISessionFactory>(provider => new WebSessionFactory(connectionFactory, provider.GetService<IHttpContextAccessor>()));
            serviceCollection.AddTransient(provider => provider.GetService<ISessionFactory>().OpenSession());
        }
    }
}
