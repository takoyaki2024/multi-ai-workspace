using MultiAiWorkspace.Providers;

namespace MultiAiWorkspace.Tests;

public sealed class ProviderUrlTests
{
    [Theory]
    [InlineData("https://chatgpt.com/", true)]
    [InlineData("https://chat.openai.com/c/1", true)]
    [InlineData("https://evilchatgpt.com/", false)]
    [InlineData("https://gemini.google.com/", false)]
    public void ChatGpt_RecognizesOnlySupportedHosts(string url, bool expected) =>
        Assert.Equal(expected, new ChatGptProvider().Supports(new Uri(url)));

    [Theory]
    [InlineData("https://gemini.google.com/app", true)]
    [InlineData("https://claude.ai/new", false)]
    public void Gemini_RecognizesSupportedHost(string url, bool expected) =>
        Assert.Equal(expected, new GeminiProvider().Supports(new Uri(url)));

    [Theory]
    [InlineData("https://claude.ai/new", true)]
    [InlineData("https://notclaude.ai/", false)]
    public void Claude_RecognizesSupportedHost(string url, bool expected) =>
        Assert.Equal(expected, new ClaudeProvider().Supports(new Uri(url)));
}
