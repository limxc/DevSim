using ApiLog.Components;
using ApiLog.Middleware;
using ApiLog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiLog.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddApiLog(this IServiceCollection services, Action<ApiLogViewerOptions>? configureOptions = null)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddAntiforgery();

        services.AddSingleton<ApiLogService>(sp => new ApiLogService(ConstNames.DBPath));

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<ApiLogViewerOptions>(_ => { });
        }

        return services;
    }

    public static WebApplication UseApiLog(this WebApplication app)
    {
        app.UseAntiforgery();

        app.UseMiddleware<ApiLoggingMiddleware>();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AllowAnonymous();

        app.MapStaticAssets();
        //app.UseStaticFiles();

        return app;
    }
}