using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Upload
{
    internal sealed class Endpoint : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("upload");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request r, CancellationToken c)
        {
            Response.State = true;
        }
    }
}