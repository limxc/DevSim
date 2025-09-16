namespace ApiLog.Middleware;

public class ApiLogViewerOptions
{
    public string[] IgnorePaths { get; set; } = Array.Empty<string>();
}