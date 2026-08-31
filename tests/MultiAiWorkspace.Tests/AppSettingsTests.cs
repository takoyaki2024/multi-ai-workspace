using MultiAiWorkspace.Configuration;

namespace MultiAiWorkspace.Tests;

public sealed class AppSettingsTests
{
    [Fact]
    public void Defaults_DefineExactlyThreeUniquePanes()
    {
        var settings = new AppSettings();
        Assert.Equal(3, settings.Panes.Count);
        Assert.Equal(new[] { "ChatGPT", "Gemini", "Claude" }, settings.Panes.Select(p => p.DisplayName));
        Assert.Equal(3, settings.Panes.Select(p => p.Id).Distinct().Count());
        Assert.Equal(3, settings.Panes.Select(p => settings.GetProfilePath(p)).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void ProfilePaths_AreBelowConfiguredRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "multi-ai-test");
        var settings = new AppSettings { ProfileRoot = root };
        Assert.All(settings.Panes, pane => Assert.StartsWith(root, settings.GetProfilePath(pane), StringComparison.OrdinalIgnoreCase));
    }
}
