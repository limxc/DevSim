using ApiLog.Models;
using LiteDB;

namespace ApiLog.Services;

public class ApiLogService : IDisposable
{
    private readonly LiteDatabase _db;
    private readonly ILiteCollection<LogContext> _collection;

    public ApiLogService(string dbPath = ConstNames.DBPath)
    {
        var folder = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        _db = new LiteDatabase(dbPath);
        _collection = _db.GetCollection<LogContext>();
        _collection.EnsureIndex(x => x.Timestamp);
        _collection.EnsureIndex(x => x.Route);
    }

    public void AddLog(LogContext log)
    {
        _collection.Insert(log);
    }

    public IEnumerable<LogContext> GetLogs(DateTime? fromDate = null, DateTime? toDate = null, string? route = null)
    {
        var query = _collection.Query();

        if (fromDate.HasValue)
            query = query.Where(x => x.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x => x.Timestamp <= toDate.Value);

        if (!string.IsNullOrWhiteSpace(route))
            query = query.Where(x => x.Route.Contains(route));

        return query.OrderByDescending(x => x.Timestamp).ToEnumerable();
    }

    public void DeleteLogs(DateTime? fromDate = null, DateTime? toDate = null, string? route = null)
    {
        var logsToDelete = GetLogs(fromDate, toDate, route).Select(x => x.Id).ToList();
        if (logsToDelete.Count > 0)
        {
            _collection.DeleteMany(x => logsToDelete.Contains(x.Id));
        }
    }

    public void DeleteAll()
    {
        _collection.DeleteAll();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}