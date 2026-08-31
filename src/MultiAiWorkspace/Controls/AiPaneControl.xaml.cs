using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Web.WebView2.Core;
using MultiAiWorkspace.Configuration;
using MultiAiWorkspace.Providers;

namespace MultiAiWorkspace.Controls;

public partial class AiPaneControl : UserControl, IWebViewScriptHost
{
    private PaneDefinition? _definition;
    private IAiProvider? _provider;
    private Func<string>? _commonInput;

    public AiPaneControl() => InitializeComponent();

    public string ProviderName => _provider?.Name ?? _definition?.DisplayName ?? "AI";
    public Uri? Source => Browser.Source;

    public async Task InitializeAsync(PaneDefinition definition, IAiProvider provider, string profilePath, Func<string> commonInput)
    {
        _definition = definition;
        _provider = provider;
        _commonInput = commonInput;
        TitleText.Text = definition.DisplayName;
        SetStatus("起動中…", false);

        try
        {
            Directory.CreateDirectory(profilePath);
            var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: profilePath);
            await Browser.EnsureCoreWebView2Async(environment);
            Browser.CoreWebView2.Settings.AreDevToolsEnabled = true;
            Browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            Browser.Source = definition.HomeUri;
        }
        catch (Exception ex)
        {
            SetStatus("起動失敗: " + ex.Message, true);
        }
    }

    public Task<string> ExecuteScriptAsync(string script)
    {
        if (Browser.CoreWebView2 is null) throw new InvalidOperationException("WebView2がまだ準備できていません。");
        return Browser.CoreWebView2.ExecuteScriptAsync(script);
    }

    public async Task PasteCommonInputAsync()
    {
        EnsureReady();
        var text = _commonInput?.Invoke() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text)) throw new InvalidOperationException("共通入力欄が空です。");
        await _provider!.SetInputAsync(this, text);
        SetStatus("貼付済み（未送信）", false);
    }

    public async Task SendCurrentInputAsync()
    {
        EnsureReady();
        await _provider!.SendAsync(this);
        SetStatus("送信しました", false);
    }

    public async Task<string> CopyLatestResponseAsync()
    {
        EnsureReady();
        var response = await _provider!.GetLatestResponseAsync(this);
        Clipboard.SetText(response);
        SetStatus("回答をコピーしました", false);
        return response;
    }

    public void ReportFailure(Exception exception) => SetStatus("失敗: " + exception.Message, true);

    private void EnsureReady()
    {
        if (_provider is null || Browser.CoreWebView2 is null) throw new InvalidOperationException("ページを準備中です。");
        if (!_provider.Supports(Source)) throw new InvalidOperationException($"{ProviderName} のページを開いてください。");
    }

    private async void Browser_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (!e.IsSuccess) { SetStatus($"読込失敗: {e.WebErrorStatus}", true); return; }
        try
        {
            var ready = _provider is not null && await _provider.IsPageReadyAsync(this);
            SetStatus(ready ? "準備完了" : "ページ表示中 / ログインしてください", false);
        }
        catch (Exception ex) { ReportFailure(ex); }
    }

    private async void Paste_Click(object sender, RoutedEventArgs e) => await ExecuteUiActionAsync(PasteCommonInputAsync);
    private async void Send_Click(object sender, RoutedEventArgs e) => await ExecuteUiActionAsync(SendCurrentInputAsync);
    private async void Copy_Click(object sender, RoutedEventArgs e) => await ExecuteUiActionAsync(async () => { await CopyLatestResponseAsync(); });

    private async Task ExecuteUiActionAsync(Func<Task> action)
    {
        try { await action(); }
        catch (Exception ex) { ReportFailure(ex); }
    }

    private void SetStatus(string message, bool error)
    {
        StatusText.Text = message;
        StatusText.Foreground = error
            ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 160, 160))
            : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(157, 226, 176));
        StatusText.ToolTip = message;
    }
}
