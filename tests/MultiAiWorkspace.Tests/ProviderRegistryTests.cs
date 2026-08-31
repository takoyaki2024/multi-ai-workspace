using MultiAiWorkspace.Providers;

namespace MultiAiWorkspace.Tests;

public sealed class ProviderRegistryTests
{
    [Theory]
    [InlineData("https://chatgpt.com/", typeof(ChatGptProvider))]
    [InlineData("https://gemini.google.com/", typeof(GeminiProvider))]
    [InlineData("https://claude.ai/", typeof(ClaudeProvider))]
    public void Find_SelectsProviderForUrl(string url, Type expectedType) =>
        Assert.IsType(expectedType, AiProviderRegistry.CreateDefault().Find(new Uri(url)));

    [Fact]
    public void Find_ReturnsNullForUnknownUrl() =>
        Assert.Null(AiProviderRegistry.CreateDefault().Find(new Uri("https://example.com/")));
}
