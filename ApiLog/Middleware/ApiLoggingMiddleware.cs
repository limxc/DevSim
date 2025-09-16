using ApiLog.Models;
using ApiLog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ApiLog.Middleware;

public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiLogService _logService;
    private readonly string[] _ignorePaths;

    public ApiLoggingMiddleware(
        RequestDelegate next,
        ApiLogService logService,
        IOptions<ApiLogViewerOptions> options)
    {
        _next = next;
        _logService = logService;
        _ignorePaths = options.Value.IgnorePaths;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Don't log requests to the ApiLogViewer itself or ignored paths
        if (_ignorePaths.Any(path => context.Request.Path.StartsWithSegments(path))
            || ConstNames.IgnorePrefixs.Any(path => context.Request.Path.Value?.StartsWith(path) ?? false)
            || ConstNames.IgnorePostfixs.Any(path => context.Request.Path.Value?.EndsWith(path) ?? false)
            )
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;

        try
        {
            // Capture the request body
            string requestBody = string.Empty;
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                using var requestReader = new StreamReader(context.Request.Body, leaveOpen: true);
                requestBody = await requestReader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Capture the response body
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            memoryStream.Position = 0;
            using var responseReader = new StreamReader(memoryStream);
            var responseBody = await responseReader.ReadToEndAsync();

            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);

            // Log the API call
            _logService.AddLog(new LogContext
            {
                Route = context.Request.Path,
                Method = context.Request.Method,
                Timestamp = DateTime.UtcNow,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                StatusCode = context.Response.StatusCode,
                Duration = stopwatch.Elapsed
            });
        }
        finally
        {
            context.Response.Body = originalBodyStream;
            stopwatch.Stop();
        }
    }
}