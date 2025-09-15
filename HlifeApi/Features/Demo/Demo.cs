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
        { Get("demo1"); AllowAnonymous(); }

        public override async Task<Results<Ok<WeatherForecast[]>, NotFound, ProblemDetails>>
        ExecuteAsync(CancellationToken ct)
        {
            await Task.CompletedTask; //simulate async work

            var count = CountService.GetCount(); CountService.AddCount();

            //condition for a not found response if (count % 3 == 1) return TypedResults.NotFound();

            //condition for a problem details response

            if (count % 3 == 2)
            {
                AddError("sim error"); return new
            FastEndpoints.ProblemDetails(ValidationFailures);
            }

            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)), Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)])).ToArray();

            // 200 ok response with a DTO
            return TypedResults.Ok(forecast);
        }
    }

    internal sealed class Endpoint_2 : Ep.Req<Request>.Res<Results<Ok<WeatherForecast[]>, ProblemDetails>>
    {
        public override void Configure()
        {
            Post("demo2");
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