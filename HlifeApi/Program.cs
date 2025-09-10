using FastEndpoints.Swagger;
using Serilog;
using Serilog.Ui.Core.Extensions;
using Serilog.Ui.SqliteDataProvider.Extensions;
using Serilog.Ui.Web.Extensions;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

var dbPath = $"{Path.Combine(AppContext.BaseDirectory, "Logs", "api.db")}";
var tableName = "ApiLog";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()   // 自动日志只显示 Warning 以上
    .WriteTo.Console()
    .CreateLogger();

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.SQLite(dbPath, tableName: tableName, rollOver: true)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSerilogUi(options =>
    options.UseSqliteServer(opts => opts.WithConnectionString($"Data Source={dbPath}").WithTable(tableName)));

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(opt =>
    {
        opt.DocumentSettings = s => s.OperationProcessors.Add(new DefValueProcessor());
    });

// Add services to the container.
builder.Services.AddSingleton<CountService>();

var app = builder.Build();
app.UseFastEndpoints()
    .UseSwaggerGen();
// Configure the HTTP request pipeline.

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/logs", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    ctx.Request.EnableBuffering();
    var reqJson = "<NotJson>";
    if (ctx.Request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
    {
        ctx.Request.Body.Position = 0;
        using var sr = new StreamReader(ctx.Request.Body, leaveOpen: true);
        reqJson = await sr.ReadToEndAsync();
        ctx.Request.Body.Position = 0;
    }

    var originalRespBody = ctx.Response.Body;
    using var respMs = new MemoryStream();
    ctx.Response.Body = respMs;
    try { await next(); }
    finally
    {
        var respJson = "<NotJson>";
        if (ctx.Response.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
        {
            respMs.Position = 0;
            using var sr = new StreamReader(respMs, leaveOpen: true);
            respJson = await sr.ReadToEndAsync();
            respMs.Position = 0;
            await respMs.CopyToAsync(originalRespBody);

            logger.Information("{Req} {ReqBody} | {RespStatus} {RespBody}",
                    ctx.Request.Method, reqJson,
                    ctx.Response.StatusCode, respJson);
        }
        ctx.Response.Body = originalRespBody;
    }
});

app.UseSerilogUi(options => options.WithRoutePrefix("logs"));

logger.Information("Api starting...");

app.Run();