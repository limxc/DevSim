namespace HlifeApi.Features.Demo
{
    internal sealed class Endpoint : EndpointWithoutRequest<Results<Ok<WeatherForecast[]>,
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
            Get("weatherforecast");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<WeatherForecast[]>, NotFound, ProblemDetails>> ExecuteAsync(CancellationToken ct)
        {
            await Task.CompletedTask; //simulate async work

            //condition for a not found response
            if (CountService.Count % 3 == 1)
                return TypedResults.NotFound();

            //condition for a problem details response

            if (CountService.Count % 3 == 2)
            {
                AddError("sim error");
                return new FastEndpoints.ProblemDetails(ValidationFailures);
            }

            var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

            CountService.Count++;

            // 200 ok response with a DTO
            return TypedResults.Ok(forecast);
        }
    }

    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}