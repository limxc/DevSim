using Bogus;

namespace Query
{
    internal sealed class Request
    {
        public string? Key { get; set; }
    }

    internal sealed class Response
    {
        public List<Archive>? Archives { get; set; }
    }
}