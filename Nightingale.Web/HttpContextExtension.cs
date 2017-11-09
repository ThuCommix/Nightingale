using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nightingale.Web
{
    public static class HttpContextExtension
    {
        private static readonly Dictionary<Guid, HttpContext> HttpContexts = new Dictionary<Guid, HttpContext>();
        private static readonly object Locker = new object();

        /// <summary>
        /// Sets a unique identifier for this http context.
        /// </summary>
        /// <param name="httpContext">The http request.</param>
        public static void SetUniqueIdentifier(this HttpContext httpContext)
        {
            lock (Locker)
            {
                httpContext.Response.OnCompleted(() => RemoveHttpContext(httpContext));

                HttpContexts.Add(Guid.NewGuid(), httpContext);
            }
        }

        /// <summary>
        /// Gets the unique identifier for this http request.
        /// </summary>
        /// <param name="httpContext">The http context.</param>
        /// <returns>Returns a unique identifier.</returns>
        public static Guid GetUniqueIdentifier(this HttpContext httpContext)
        {
            lock (Locker)
            {
                if (!HttpContexts.ContainsValue(httpContext))
                    return Guid.Empty;

                return HttpContexts.FirstOrDefault(x => x.Value == httpContext).Key;
            }
        }

        private static Task RemoveHttpContext(HttpContext httpContext)
        {
            return Task.Run(() =>
            {
                var guid = HttpContexts.FirstOrDefault(x => x.Value == httpContext).Key;
                if (guid != null)
                {
                    HttpContexts.Remove(guid);
                }
            });
        }
    }
}
