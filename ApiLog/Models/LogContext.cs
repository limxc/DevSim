using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiLog.Models;

public class LogContext
{
    public int Id { get; set; }
    public string Route { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string RequestBody { get; set; } = string.Empty;
    public string ResponseBody { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public TimeSpan Duration { get; set; }

    public bool HasJsonRequest
    {
        get
        {
            if (string.IsNullOrWhiteSpace(RequestBody))
                return false;

            ReadOnlySpan<char> trimmed = RequestBody.AsSpan().Trim();
            if (!(trimmed.StartsWith("{") || trimmed.StartsWith("[")))
                return false;

            try
            {
                JsonDocument.Parse(RequestBody);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public bool HasJsonResponse
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ResponseBody))
                return false;

            ReadOnlySpan<char> trimmed = ResponseBody.AsSpan().Trim();
            if (!(trimmed.StartsWith("{") || trimmed.StartsWith("[")))
                return false;

            try
            {
                JsonDocument.Parse(ResponseBody);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public JsonNode? GetRequestJson() => HasJsonRequest ? JsonNode.Parse(RequestBody) : null;

    public JsonNode? GetResponseJson() => HasJsonResponse ? JsonNode.Parse(ResponseBody) : null;
}