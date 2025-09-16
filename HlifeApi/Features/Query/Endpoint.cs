using FluentValidation.Results;
using Upload;

namespace Query
{
    internal sealed class Endpoint : Endpoint<Request, Archive[]>
    {
        public override void Configure()
        {
            Post("query");
            AllowAnonymous();
            Version(0);
        }

        public override async Task HandleAsync(Request r, CancellationToken c)
        {
            await Task.CompletedTask;

            var key = r.Key;

            List<Archive> rst;
            if (string.IsNullOrWhiteSpace(key))
            {
                rst = Archive.Mock.Generate(10).ToList();
            }
            else
            {
                rst = Archive.Mock.Generate(10000).Where(p => p.Name.Contains(key) || p.CardId.Contains(key)).Take(10).ToList();
            }

            Response = rst.ToArray();
        }
    }

    internal sealed class Endpoint_1 : Endpoint<Request, Results<Ok<Archive[]>, NotFound>>
    {
        public required CountService CountService { get; set; }

        public override void Configure()
        {
            Post("query");
            AllowAnonymous();
            Version(1);
        }

        public override async Task HandleAsync(Request r, CancellationToken c)
        {
            var count = CountService.GetCount();
            CountService.AddCount();

            //condition for a not found response
            if (count % 3 == 1)
            {
                await Send.ResultAsync(TypedResults.NotFound());
                return;
            }

            //condition for a problem details response
            if (count % 3 == 2)
            {
                AddError("sim error");

                await Send.ResultAsync(new FastEndpoints.ProblemDetails(ValidationFailures));
                return;
            }

            var key = r.Key;

            List<Archive> rst;
            if (string.IsNullOrWhiteSpace(key))
                rst = Archive.Mock.Generate(10).ToList();
            else
                rst = Archive.Mock.Generate(10000).Where(p => p.Name.Contains(key) || p.CardId.Contains(key)).Take(10).ToList();

            await Send.ResultAsync(TypedResults.Ok<Archive[]>(rst.ToArray()));
        }
    }
}