namespace MultiAiWorkspace.Providers;

public sealed class GeminiProvider : DomAiProviderBase
{
    public override string Name => "Gemini";
    protected override IReadOnlyList<string> Hosts => ["gemini.google.com"];
    protected override IReadOnlyList<string> InputSelectors => [".ql-editor[contenteditable='true']", "rich-textarea [contenteditable='true']", "div[contenteditable='true'][role='textbox']", "textarea"];
    protected override IReadOnlyList<string> SendSelectors => ["button[aria-label*='Send']", "button[aria-label*='送信']", "button.send-button", "button[data-test-id*='send']"];
    protected override IReadOnlyList<string> ResponseSelectors => ["model-response .markdown", "message-content .markdown", ".model-response-text", "main model-response"];
}
