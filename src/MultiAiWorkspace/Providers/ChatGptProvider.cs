namespace MultiAiWorkspace.Providers;

public sealed class ChatGptProvider : DomAiProviderBase
{
    public override string Name => "ChatGPT";
    protected override IReadOnlyList<string> Hosts => ["chatgpt.com", "chat.openai.com"];
    protected override IReadOnlyList<string> InputSelectors => ["#prompt-textarea", "textarea[data-id='root']", "div[contenteditable='true'][role='textbox']", "textarea", "[contenteditable='true']"];
    protected override IReadOnlyList<string> SendSelectors => ["button[data-testid='send-button']", "button[aria-label*='Send']", "button[aria-label*='送信']", "form button[type='submit']"];
    protected override IReadOnlyList<string> ResponseSelectors => ["[data-message-author-role='assistant']", "article[data-testid*='conversation-turn'] .markdown", "main article .markdown"];
}
