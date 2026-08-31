namespace MultiAiWorkspace.Providers;

public sealed class AiProviderRegistry
{
    private readonly IReadOnlyList<IAiProvider> _providers;

    public AiProviderRegistry(IEnumerable<IAiProvider> providers) => _providers = providers.ToArray();

    public IAiProvider? Find(Uri? uri) => _providers.FirstOrDefault(provider => provider.Supports(uri));

    public IAiProvider GetByName(string name) => _providers.FirstOrDefault(provider =>
        provider.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? throw new KeyNotFoundException($"Provider '{name}' は登録されていません。");

    public static AiProviderRegistry CreateDefault() => new IAiProvider[]
    {
        new ChatGptProvider(), new GeminiProvider(), new ClaudeProvider()
    }.ToRegistry();
}

internal static class ProviderExtensions
{
    public static AiProviderRegistry ToRegistry(this IEnumerable<IAiProvider> providers) => new(providers);
}
