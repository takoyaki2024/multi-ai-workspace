using MultiAiWorkspace.Services;

namespace MultiAiWorkspace.Tests;

public sealed class IsolatedOperationRunnerTests
{
    [Fact]
    public async Task RunAllAsync_ContinuesWhenOneProviderFails()
    {
        var called = new List<string>();
        var operations = new (string Name, Func<Task<int>> Operation)[]
        {
            ("ChatGPT", () => { called.Add("ChatGPT"); return Task.FromResult(1); }),
            ("Gemini", () => throw new InvalidOperationException("DOM changed")),
            ("Claude", () => { called.Add("Claude"); return Task.FromResult(3); })
        };

        var results = await IsolatedOperationRunner.RunAllAsync(operations);

        Assert.Equal(3, results.Count);
        Assert.Equal(2, results.Count(result => result.Succeeded));
        Assert.Contains("ChatGPT", called);
        Assert.Contains("Claude", called);
        Assert.IsType<InvalidOperationException>(results.Single(result => result.Name == "Gemini").Error);
    }
}
