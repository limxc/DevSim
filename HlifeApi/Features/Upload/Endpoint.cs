using Query;

namespace Upload
{
    internal sealed class Endpoint : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("upload");
            AllowAnonymous();
            Version(0);
        }

        public override async Task HandleAsync(Request r, CancellationToken c)
        {
            Response.State = true;
        }
    }

    internal sealed class Endpoint_1 : Ep.Req<Request>.Res<Results<Ok<Response>, ProblemDetails>>
    {
        public required CountService CountService { get; set; }

        public override void Configure()
        {
            Post("upload");
            AllowAnonymous();
            Version(1);
        }

        public override async Task<Results<Ok<Response>, ProblemDetails>> ExecuteAsync(Request r, CancellationToken c)
        {
            var count = CountService.GetCount();
            CountService.AddCount();

            if (count % 2 == 0)
            {
                AddError("sim error");
                return new FastEndpoints.ProblemDetails(ValidationFailures);
            }

            return TypedResults.Ok(new Response() { State = true });
        }
    }
}