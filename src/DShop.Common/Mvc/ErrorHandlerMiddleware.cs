using System;
using System.Net;
using System.Threading.Tasks;
using DShop.Common.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DShop.Common.Mvc
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, 
            ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                await HandleErrorAsync(context, exception);
            }
        }

        private static Task HandleErrorAsync(HttpContext context, Exception exception)
        {
            var errorCode = "error";
            var statusCode = HttpStatusCode.BadRequest;
            var message = "There was an error.";
            switch(exception)
            {
                case DShopException e:
                    errorCode = e.Code;
                    message = e.Message;
                    break;
            }
            var response = new { code = errorCode, message = message };
            var payload = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(payload);
        }
    }
}
