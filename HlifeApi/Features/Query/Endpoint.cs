namespace Query
{
    internal sealed class Endpoint : Endpoint<Request, Archive[]>
    {
        public override void Configure()
        {
            Post("query");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request r, CancellationToken c)
        {
            var key = r.Key;

            List<Archive> rst;
            if (string.IsNullOrWhiteSpace(key))
                rst = Archive.Mock.Generate(10).ToList();
            else
                rst = Archive.Mock.Generate(10000).Where(p => p.Name.Contains(key) || p.CardId.Contains(key)).Take(10).ToList();

            Response = rst.ToArray();
        }
    }
}