namespace HlifeApi.Features.Demo
{
    internal sealed class Endpoint_1 : EndpointWithoutRequest<Results<Ok<WeatherForecast[]>,
                                           NotFound,
                                           ProblemDetails>>
    {
        private string[] summaries =
                [
                    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                ];

        public required CountService CountService { get; set; }

        public override void Configure()
        {
            Get("/Demo/demo1");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<WeatherForecast[]>, NotFound, ProblemDetails>>
        ExecuteAsync(CancellationToken ct)
        {
            await Task.CompletedTask;

            var count = CountService.GetCount(); CountService.AddCount();

            if (count % 3 == 2)
            {
                AddError("sim error");
                return new ProblemDetails(ValidationFailures);
            }

            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)), Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)])).ToArray();

            return TypedResults.Ok(forecast);
        }
    }

    internal sealed class Endpoint_2 : Ep.Req<Request>.Res<Results<Ok<WeatherForecast[]>, ProblemDetails>>
    {
        public override void Configure()
        {
            Post("/Demo/demo2");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<WeatherForecast[]>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
        {
            await Task.CompletedTask;
            if (DateTime.Now.Second % 3 == 1)
            {
                AddError("sim error");
                return new ProblemDetails(ValidationFailures);
            }

            return TypedResults.Ok(new WeatherForecast[]
            {
                new WeatherForecast(new DateOnly(2000,1,1), 25, ""),
                new WeatherForecast(new DateOnly(2001,2,2), 33, ""),
            });
        }
    }
}