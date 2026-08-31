using System.Text.Json;

namespace MultiAiWorkspace.Providers;

public abstract class DomAiProviderBase : IAiProvider
{
    protected abstract IReadOnlyList<string> Hosts { get; }
    protected abstract IReadOnlyList<string> InputSelectors { get; }
    protected abstract IReadOnlyList<string> SendSelectors { get; }
    protected abstract IReadOnlyList<string> ResponseSelectors { get; }

    public abstract string Name { get; }

    public bool Supports(Uri? uri) => uri is not null && Hosts.Any(host =>
        uri.Host.Equals(host, StringComparison.OrdinalIgnoreCase) ||
        uri.Host.EndsWith('.' + host, StringComparison.OrdinalIgnoreCase));

    public async Task<bool> IsPageReadyAsync(IWebViewScriptHost webView)
    {
        if (!Supports(webView.Source)) return false;
        var result = await RunAsync(webView, BuildFindScript(InputSelectors, "return !!element;"));
        return result.Ok && string.Equals(result.Value, "true", StringComparison.OrdinalIgnoreCase);
    }

    public async Task SetInputAsync(IWebViewScriptHost webView, string text)
    {
        EnsureSupported(webView);
        var encoded = JsonSerializer.Serialize(text);
        var body = $$"""
            const value = {{encoded}};
            element.focus();
            if (element instanceof HTMLTextAreaElement || element instanceof HTMLInputElement) {
              const proto = element instanceof HTMLTextAreaElement ? HTMLTextAreaElement.prototype : HTMLInputElement.prototype;
              const setter = Object.getOwnPropertyDescriptor(proto, 'value')?.set;
              setter ? setter.call(element, value) : element.value = value;
            } else {
              element.textContent = value;
            }
            element.dispatchEvent(new InputEvent('input', { bubbles: true, inputType: 'insertText', data: value }));
            element.dispatchEvent(new Event('change', { bubbles: true }));
            return true;
            """;
        var result = await RunAsync(webView, BuildFindScript(InputSelectors, body));
        EnsureSuccess(result, "入力欄が見つからないか、テキストを投入できませんでした。");
    }

    public async Task SendAsync(IWebViewScriptHost webView)
    {
        EnsureSupported(webView);
        var click = await RunAsync(webView, BuildFindScript(SendSelectors, "if (element.disabled || element.getAttribute('aria-disabled') === 'true') throw new Error('送信ボタンが無効です'); element.click(); return true;"));
        if (click.Ok) return;

        var fallback = await RunAsync(webView, BuildFindScript(InputSelectors, "element.focus(); element.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', code: 'Enter', bubbles: true, cancelable: true })); element.dispatchEvent(new KeyboardEvent('keyup', { key: 'Enter', code: 'Enter', bubbles: true, cancelable: true })); return true;"));
        EnsureSuccess(fallback, "送信操作に失敗しました。入力内容やログイン状態を確認してください。");
    }

    public async Task<string> GetLatestResponseAsync(IWebViewScriptHost webView)
    {
        EnsureSupported(webView);
        var result = await RunAsync(webView, BuildFindAllScript(ResponseSelectors));
        EnsureSuccess(result, "最新の回答が見つかりませんでした。");
        if (string.IsNullOrWhiteSpace(result.Value)) throw new InvalidOperationException("最新の回答が空です。");
        return result.Value;
    }

    private void EnsureSupported(IWebViewScriptHost webView)
    {
        if (!Supports(webView.Source)) throw new InvalidOperationException($"{Name} のページが開かれていません。");
    }

    private static void EnsureSuccess(ScriptResult result, string fallbackMessage)
    {
        if (!result.Ok) throw new InvalidOperationException(result.Error ?? fallbackMessage);
    }

    private static string BuildFindScript(IEnumerable<string> selectors, string body)
    {
        var encoded = JsonSerializer.Serialize(selectors);
        return $$"""
            (() => { try {
              const selectors = {{encoded}};
              const visible = e => e && e.getClientRects().length > 0;
              let element = selectors.map(s => document.querySelector(s)).find(visible);
              if (!element) throw new Error('対象要素が見つかりません');
              {{body}}
            } catch (e) { return { ok: false, error: String(e?.message ?? e) }; } })()
            """;
    }

    private static string BuildFindAllScript(IEnumerable<string> selectors)
    {
        var encoded = JsonSerializer.Serialize(selectors);
        return $$"""
            (() => { try {
              const selectors = {{encoded}};
              const elements = [...new Set(selectors.flatMap(s => [...document.querySelectorAll(s)]))]
                .filter(e => e.getClientRects().length > 0);
              const element = elements.at(-1);
              if (!element) throw new Error('回答要素が見つかりません');
              return { ok: true, value: (element.innerText || element.textContent || '').trim() };
            } catch (e) { return { ok: false, error: String(e?.message ?? e) }; } })()
            """;
    }

    private static async Task<ScriptResult> RunAsync(IWebViewScriptHost webView, string script)
    {
        var raw = await webView.ExecuteScriptAsync(script);
        if (string.IsNullOrWhiteSpace(raw) || raw == "null") return new(false, null, "ページから応答がありません。");
        try
        {
            using var document = JsonDocument.Parse(raw);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.String) return Parse(root);
            using var innerDocument = JsonDocument.Parse(root.GetString() ?? "null");
            return Parse(innerDocument.RootElement);
        }
        catch (JsonException ex) { throw new InvalidOperationException("ページ操作の応答を解析できませんでした。", ex); }
    }

    private static ScriptResult Parse(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.True || root.ValueKind == JsonValueKind.False)
            return new(true, root.GetBoolean().ToString().ToLowerInvariant(), null);
        var ok = root.TryGetProperty("ok", out var okNode) && okNode.GetBoolean();
        var value = root.TryGetProperty("value", out var valueNode) ? valueNode.GetString() : null;
        var error = root.TryGetProperty("error", out var errorNode) ? errorNode.GetString() : null;
        return new(ok, value, error);
    }
}
