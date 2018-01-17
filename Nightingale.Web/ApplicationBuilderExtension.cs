using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace Nightingale.Web
{
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// Uses nightingale in the http pipeline.
        /// </summary>
        /// <param name="applicationBuilder">The application builder.</param>
        public static void UseNightingale(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<NightingaleMiddleware>();
        }
    }
}
