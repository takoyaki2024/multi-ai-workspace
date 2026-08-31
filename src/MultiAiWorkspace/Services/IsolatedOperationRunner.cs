namespace MultiAiWorkspace.Services;

public sealed record OperationOutcome<T>(string Name, bool Succeeded, T? Value, Exception? Error);

public static class IsolatedOperationRunner
{
    public static async Task<IReadOnlyList<OperationOutcome<T>>> RunAllAsync<T>(
        IEnumerable<(string Name, Func<Task<T>> Operation)> operations)
    {
        var tasks = operations.Select(async item =>
        {
            try { return new OperationOutcome<T>(item.Name, true, await item.Operation(), null); }
            catch (Exception ex) { return new OperationOutcome<T>(item.Name, false, default, ex); }
        });
        return await Task.WhenAll(tasks);
    }
}
