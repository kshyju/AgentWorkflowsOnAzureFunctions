using Microsoft.Agents.AI.Workflows;

namespace AgentsAndWorkflowsFunctions;

internal sealed class TranslateText() : Executor<string, TranslationResult>("TranslateText")
{
    public override ValueTask<TranslationResult> HandleAsync(
        string message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[Activity] TranslateText: '{message}'");
        return ValueTask.FromResult(new TranslationResult(message, message.ToUpperInvariant()));
    }
}

internal sealed class FormatOutput() : Executor<TranslationResult, string>("FormatOutput")
{
    public override ValueTask<string> HandleAsync(
        TranslationResult message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[Activity] FormatOutput: Formatting result");
        return ValueTask.FromResult($"Original: {message.Original} => Translated: {message.Translated}");
    }
}

internal sealed class ReverseText() : Executor<TranslationResult, string>("ReverseText")
{
    public override ValueTask<string> HandleAsync(
        TranslationResult message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string reversed = new(message.Translated.Reverse().ToArray());
        Console.WriteLine($"[Activity] ReverseText: '{message.Translated}' => '{reversed}'");
        return ValueTask.FromResult(reversed);
    }
}

internal sealed record TranslationResult(string Original, string Translated);
