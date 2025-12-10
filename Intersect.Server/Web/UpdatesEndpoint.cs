using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Intersect.Server.Core.Updates;

namespace Intersect.Server.Web
{
    public static class UpdatesEndpoint
    {
        public static void RegisterRoutes(IApplicationBuilder app)
        {
            app.Map("/api/v1/updates/upload", builder => builder.Run(HandleUpload));
            app.Map("/api/v1/updates/manifest", builder => builder.Run(HandleManifest));
            app.Map("/api/v1/updates/files", builder => builder.Run(HandleFiles));
        }

        private static async Task HandleUpload(HttpContext context)
        {
            // Same implementation as HandleUpdateUpload above
        }

        private static async Task HandleManifest(HttpContext context)
        {
            // Same implementation as HandleGetManifest above
        }

        private static async Task HandleFiles(HttpContext context)
        {
            // Same implementation as HandleGetUpdateFile above
        }
    }
}
