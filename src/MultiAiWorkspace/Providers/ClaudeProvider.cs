namespace MultiAiWorkspace.Providers;

public sealed class ClaudeProvider : DomAiProviderBase
{
    public override string Name => "Claude";
    protected override IReadOnlyList<string> Hosts => ["claude.ai"];
    protected override IReadOnlyList<string> InputSelectors => ["div.ProseMirror[contenteditable='true']", "[contenteditable='true'][role='textbox']", "fieldset [contenteditable='true']", "textarea"];
    protected override IReadOnlyList<string> SendSelectors => ["button[aria-label*='Send']", "button[aria-label*='送信']", "button[data-testid*='send']", "fieldset button[type='button']:last-of-type"];
    protected override IReadOnlyList<string> ResponseSelectors => ["[data-is-streaming] .font-claude-message", ".font-claude-message", "[data-testid*='assistant']", "main article"];
}
