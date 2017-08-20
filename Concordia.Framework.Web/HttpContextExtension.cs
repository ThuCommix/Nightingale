using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Concordia.Framework.Web
{
    public static class HttpContextExtension
    {
        private static readonly Dictionary<Guid, HttpContext> _httpContexts = new Dictionary<Guid, HttpContext>();
        private static readonly object _locker = new object();

        /// <summary>
        /// Sets a unique identifier for this http context.
        /// </summary>
        /// <param name="httpRequest">The http request.</param>
        public static void SetUniqueIdentifier(this HttpContext httpContext)
        {
            lock (_locker)
            {
                httpContext.Response.OnCompleted(() => RemoveHttpContext(httpContext));

                _httpContexts.Add(Guid.NewGuid(), httpContext);
            }
        }

        /// <summary>
        /// Gets the unique identifier for this http request.
        /// </summary>
        /// <param name="httpRequest">The http context.</param>
        /// <returns>Returns a unique identifier.</returns>
        public static Guid GetUniqueIdentifier(this HttpContext httpContext)
        {
            lock (_locker)
            {
                if (!_httpContexts.ContainsValue(httpContext))
                    return Guid.Empty;

                return _httpContexts.FirstOrDefault(x => x.Value == httpContext).Key;
            }
        }

        private static Task RemoveHttpContext(HttpContext httpContext)
        {
            return Task.Run(() =>
            {
                var guid = _httpContexts.FirstOrDefault(x => x.Value == httpContext).Key;
                if (guid != null)
                {
                    _httpContexts.Remove(guid);
                }
            });
        }
    }
}
