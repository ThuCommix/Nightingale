using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nightingale.Web
{
    /// <summary>
    /// Responsible for disposing created <see cref="ISession"/> after finishing the request.
    /// </summary>
    public class NightingaleMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new NightingaleMiddleware class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        public NightingaleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes this middleware.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>Returns a task instance.</returns>
        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);

            if (context.Items.ContainsKey(WebSessionFactory.SessionId))
            {
                var session = (Sessions.ISession)context.Items[WebSessionFactory.SessionId];
                if (session.Connection.IsOpen)
                {
                    session.Dispose();
                }
            }
        }
    }
}
