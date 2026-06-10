using static Google.Apis.Requests.RequestError;
using System.Net;
using System.Text.Json;
using Maintenance.Entities.Responses;

namespace Maintenance.Middleware
{
    public class VersionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public VersionValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ignorePaths = new[]
            {
                "/api/b1cs/v1/auth/appSignIn",
                "/api/b1cs/v1/auth/signIn",
                "/api/b1cs/v1/auth/lockUser",
                "/api/b1cs/v1/auth/unLockUser",
                "/hangfire",
                "/job-dashboard"
            };


            var path = context.Request.Path.Value?.ToLower();

            if (ignorePaths.Any(p => path.StartsWith(p.ToLower())))
            {
                await _next(context);
                return;
            }

            var versionFromToken = context.User.FindFirst("Version")?.Value;
            var validVersion = _configuration["AppSettings:ValidVersion"];

            if (!string.IsNullOrEmpty(validVersion) && versionFromToken != validVersion)
            {
                context.Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                context.Response.ContentType = "application/json";

                var response = ResponseApi<bool>.Error(
                    errorCode: ErrorCode.InvalidVersion,
                    message: "Your app version is not supported. Please update to the latest version"
                );

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
                return;
            }

            await _next(context);
        }
    }
}
