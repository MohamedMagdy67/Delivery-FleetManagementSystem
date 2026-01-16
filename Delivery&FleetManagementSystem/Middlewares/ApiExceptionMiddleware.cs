using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Delivery_FleetManagementSystem.Middlewares
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode status;

            // تحديد نوع الحالة بناءً على Exception
            switch (ex)
            {
                case UnauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    break;
                case ArgumentException:
                case InvalidOperationException:
                    status = HttpStatusCode.BadRequest;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.StatusCode = (int)status;

            var response = new
            {
                status = context.Response.StatusCode,
                message = ex.Message,
                details = ex.InnerException?.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
