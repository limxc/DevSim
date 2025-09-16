using ApiLog.Components;
using ApiLog.Middleware;
using ApiLog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ApiLog.Extensions;

public static class ServiceExtension
{
    // 在 builder 阶段注册服务
    public static IServiceCollection AddApiLog(this IServiceCollection services, Action<ApiLogViewerOptions>? configureOptions = null)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();

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

    // 在 app 阶段挂中间件和页面
    public static WebApplication UseApiLog(this WebApplication app)
    {
        app.UseAntiforgery();

        app.UseMiddleware<ApiLoggingMiddleware>();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AllowAnonymous();

        //app.MapStaticAssets();
        app.UseStaticFiles();

        return app;
    }
}