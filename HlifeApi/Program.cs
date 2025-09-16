using ApiLog.Extensions;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiLog(options =>
{
    options.IgnorePaths = new[] { "/" };
});
builder.Services.AddSingleton<CountService>();

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(opt =>
    {
        opt.DocumentSettings = s =>
        {
            s.OperationProcessors.Add(new SwaggerDefaultValueProcessor());
            s.DocumentName = "Default";
            s.Version = "v0";
        };
    })
    .SwaggerDocument(opt =>
    {
        opt.MaxEndpointVersion = 1;
        opt.DocumentSettings = s =>
        {
            s.OperationProcessors.Add(new SwaggerDefaultValueProcessor());
            s.DocumentName = "v1";
            s.Version = "v1";
        };
    });

var app = builder.Build();

app.UseFastEndpoints(c =>
    {
        c.Versioning.Prefix = "v";
        c.Versioning.DefaultVersion = 0;
    })
    .UseSwaggerGen();

app.UseApiLog();

app.Run();