using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Concordia.Framework.Web
{
    internal static class HttpContextExtension
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
                ClearHttpRequests();

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
                ClearHttpRequests();

                if (!_httpContexts.ContainsValue(httpContext))
                    return Guid.Empty;

                return _httpContexts.FirstOrDefault(x => x.Value == httpContext).Key;
            }
        }

        /// <summary>
        /// Clears http requests which are not valid anymore.
        /// </summary>
        private static void ClearHttpRequests()
        {
            foreach(var httpRequestEntry in _httpContexts.ToList())
            {
                if(!httpRequestEntry.Value?.Response?.IsClientConnected ?? false)
                {
                    _httpContexts.Remove(httpRequestEntry.Key);
                }
            }
        }
    }
}
