using System.IO;

namespace MultiAiWorkspace.Configuration;

public sealed record PaneDefinition(string Id, string DisplayName, Uri HomeUri, string ProfileFolderName);

public sealed class AppSettings
{
    public string ProductName { get; init; } = "Multi AI Workspace";

    public string ProfileRoot { get; init; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MultiAiWorkspace", "WebViewProfiles");

    public IReadOnlyList<PaneDefinition> Panes { get; init; } =
    [
        new("chatgpt", "ChatGPT", new Uri("https://chatgpt.com/"), "chatgpt"),
        new("gemini", "Gemini", new Uri("https://gemini.google.com/"), "gemini"),
        new("claude", "Claude", new Uri("https://claude.ai/"), "claude")
    ];

    public string GetProfilePath(PaneDefinition pane) => Path.Combine(ProfileRoot, pane.ProfileFolderName);
}
