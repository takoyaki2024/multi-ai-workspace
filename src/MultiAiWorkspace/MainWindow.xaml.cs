using System.Windows;
using System.Windows.Input;
using MultiAiWorkspace.Configuration;
using MultiAiWorkspace.Controls;
using MultiAiWorkspace.Providers;
using MultiAiWorkspace.Services;

namespace MultiAiWorkspace;

public partial class MainWindow : Window
{
    private readonly AppSettings _settings = new();
    private readonly AiProviderRegistry _providers = AiProviderRegistry.CreateDefault();
    private bool _fullscreen;
    private WindowState _previousState;
    private WindowStyle _previousStyle;

    private IReadOnlyList<AiPaneControl> Panes => [ChatGptPane, GeminiPane, ClaudePane];

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var mappings = new[]
        {
            (Pane: ChatGptPane, Definition: _settings.Panes[0]),
            (Pane: GeminiPane, Definition: _settings.Panes[1]),
            (Pane: ClaudePane, Definition: _settings.Panes[2])
        };
        await Task.WhenAll(mappings.Select(item => item.Pane.InitializeAsync(
            item.Definition,
            _providers.GetByName(item.Definition.DisplayName),
            _settings.GetProfilePath(item.Definition),
            () => CommonInput.Text)));
    }

    private async void PasteAll_Click(object sender, RoutedEventArgs e) =>
        await RunForAllAsync("貼付", pane => pane.PasteCommonInputAsync());

    private async void SendAll_Click(object sender, RoutedEventArgs e) =>
        await RunForAllAsync("送信", pane => pane.SendCurrentInputAsync());

    private async Task RunForAllAsync(string operationName, Func<AiPaneControl, Task> operation)
    {
        var operations = Panes.Select<AiPaneControl, (string Name, Func<Task<bool>> Operation)>(pane =>
            (pane.ProviderName, async () => { await operation(pane); return true; })).ToArray();
        var outcomes = await IsolatedOperationRunner.RunAllAsync(operations);
        foreach (var failure in outcomes.Where(outcome => !outcome.Succeeded))
            Panes.First(pane => pane.ProviderName == failure.Name).ReportFailure(failure.Error!);

        var failed = outcomes.Count(outcome => !outcome.Succeeded);
        if (failed > 0)
            Title = $"Multi AI Workspace — {operationName}: {outcomes.Count - failed}件成功 / {failed}件失敗";
        else
            Title = $"Multi AI Workspace — すべて{operationName}しました";
    }

    private void Equalize_Click(object sender, RoutedEventArgs e)
    {
        LeftColumn.Width = new GridLength(1, GridUnitType.Star);
        CenterColumn.Width = new GridLength(1, GridUnitType.Star);
        RightColumn.Width = new GridLength(1, GridUnitType.Star);
    }

    private void Fullscreen_Click(object sender, RoutedEventArgs e) => ToggleFullscreen();

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.F11) return;
        ToggleFullscreen();
        e.Handled = true;
    }

    private void ToggleFullscreen()
    {
        if (!_fullscreen)
        {
            _previousState = WindowState;
            _previousStyle = WindowStyle;
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            FullscreenButton.Content = "全画面を終了 (F11)";
        }
        else
        {
            WindowStyle = _previousStyle;
            WindowState = _previousState;
            FullscreenButton.Content = "全画面 (F11)";
        }
        _fullscreen = !_fullscreen;
    }
}
