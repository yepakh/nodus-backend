using Grpc.Core;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting.Internal;
using Nodus.gRPC.ExceptionHandler.CustomExceptions;
using Sentry;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Nodus.gRPC.ExceptionHandler
{
    public static class ErrorHandlingAspNetCoreBuilder
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            VerifyServicesRegistered(app);

            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }

        private static void VerifyServicesRegistered(IApplicationBuilder app)
        {
            if (app.ApplicationServices.GetService(typeof(GlobalExceptionHandlerMiddleware)) == null)
            {
                throw new InvalidOperationException(
                    $"Unable to find the required services. Please add all the required services by calling 'IServiceCollection.{nameof(ErrorHandlingDependencyInjection.AddErrorHandlingServices)}' inside the call to 'ConfigureServices(...)' in the application startup code."
                );
            }
        }
    }

    public static class ErrorHandlingDependencyInjection
    {
        public static IServiceCollection AddErrorHandlingServices(this IServiceCollection services)
        {
            return services.AddSingleton<GlobalExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionResponse
    {
        public CustomExceptionResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private const String ErrorMessage = @"{""code"":500,""message"":""Internal server error.""}";
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _webHostEnvironment = env;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (RpcException ex)
            {
                switch (ex.StatusCode)
                {
                    case StatusCode.NotFound:
                        await HandleNotFoundExceptionAsync(context, ex.Status.Detail);
                        break;

                    case StatusCode.InvalidArgument:
                        await HandleBadRequestExceptionAsync(context, ex.Status.Detail);
                        break;

                    case StatusCode.Internal:
                        if(ex.Status.Detail == "Sequence contains no elements.")
                        {
                            await HandleNotFoundExceptionAsync(context, ex.Status.Detail);
                            break;
                        }

                        goto default;

                    default:
                        _logger.LogError(ex, "Error when process [{Method}] request to [{EncodedUrl}] ", context.Request.Method, context.Request.GetEncodedUrl());
                        SentrySdk.CaptureException(ex);
                        await HandleExceptionAsync(context, ex);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when process [{Method}] request to [{EncodedUrl}] ", context.Request.Method, context.Request.GetEncodedUrl());
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                throw ex;
            }
            else
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return context.Response.WriteAsync(ErrorMessage);
            }
        }

        private Task HandleNotFoundExceptionAsync(HttpContext context, string message)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return context.Response.WriteAsJsonAsync(new CustomExceptionResponse(404, message));
        }

        private Task HandleBadRequestExceptionAsync(HttpContext context, string message)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.Response.WriteAsJsonAsync(new CustomExceptionResponse(400, message));
        }
    }
}

