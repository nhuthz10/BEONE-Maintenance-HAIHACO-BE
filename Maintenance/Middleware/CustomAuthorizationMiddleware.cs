using System.Text.Json;

namespace Maintenance.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.HasStarted)
                return;

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized ||
                context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    errCode = context.Response.StatusCode,
                    message = context.Response.StatusCode == 401
                        ? "Unauthorized"
                        : "You do not have permission to access this resource"
                };

                var json = JsonSerializer.Serialize(result);

                await context.Response.WriteAsync(json);
            }
        }

    }
}
