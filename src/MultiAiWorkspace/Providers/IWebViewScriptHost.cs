namespace MultiAiWorkspace.Providers;

public interface IWebViewScriptHost
{
    Uri? Source { get; }
    Task<string> ExecuteScriptAsync(string script);
}
