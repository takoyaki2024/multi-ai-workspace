namespace MultiAiWorkspace.Providers;

public interface IAiProvider
{
    string Name { get; }
    bool Supports(Uri? uri);
    Task<bool> IsPageReadyAsync(IWebViewScriptHost webView);
    Task SetInputAsync(IWebViewScriptHost webView, string text);
    Task SendAsync(IWebViewScriptHost webView);
    Task<string> GetLatestResponseAsync(IWebViewScriptHost webView);
}
