using Microsoft.Agents.AI.Workflows;

namespace NestedWorkflowsFunctions;

/// <summary>Adds a prefix to the input text.</summary>
internal sealed class PrefixExecutor(string prefix) : Executor<string, string>("PrefixExecutor")
{
    public override ValueTask<string> HandleAsync(
        string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(prefix + message);
    }
}

/// <summary>Converts input text to uppercase.</summary>
internal sealed class UppercaseExecutor() : Executor<string, string>("UppercaseExecutor")
{
    public override ValueTask<string> HandleAsync(
        string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(message.ToUpperInvariant());
    }
}

/// <summary>Reverses the input text.</summary>
internal sealed class ReverseExecutor() : Executor<string, string>("ReverseExecutor")
{
    public override ValueTask<string> HandleAsync(
        string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(string.Concat(message.Reverse()));
    }
}

/// <summary>Appends a suffix to the input text.</summary>
internal sealed class AppendSuffixExecutor(string suffix) : Executor<string, string>("AppendSuffixExecutor")
{
    public override ValueTask<string> HandleAsync(
        string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(message + suffix);
    }
}

/// <summary>Performs final post-processing by wrapping the text.</summary>
internal sealed class PostProcessExecutor() : Executor<string, string>("PostProcessExecutor")
{
    public override ValueTask<string> HandleAsync(
        string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult($"[FINAL] {message} [END]");
    }
}
