using Microsoft.Agents.AI.Workflows;

namespace FanInFanoutFunctions;

/// <summary>
/// Parses and validates the incoming question before sending to AI agents.
/// </summary>
internal sealed class ParseQuestionExecutor() : Executor<string, string>("ParseQuestion")
{
    public override ValueTask<string> HandleAsync(
        string message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string formattedQuestion = message.Trim();
        if (!formattedQuestion.EndsWith('?'))
        {
            formattedQuestion += "?";
        }

        return ValueTask.FromResult(formattedQuestion);
    }
}

/// <summary>
/// Aggregates responses from all AI agents into a comprehensive answer.
/// This is the fan-in point where parallel results are collected.
/// </summary>
internal sealed class AggregatorExecutor() : Executor<string[], string>("Aggregator")
{
    public override ValueTask<string> HandleAsync(
        string[] message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string aggregatedResult = "AI EXPERT PANEL RESPONSES\n" +
                                 "═══════════════════════════════════════\n\n";

        for (int i = 0; i < message.Length; i++)
        {
            string expertLabel = i == 0 ? "PHYSICIST" : "CHEMIST";
            aggregatedResult += $"{expertLabel}:\n{message[i]}\n\n";
        }

        aggregatedResult += "═══════════════════════════════════════\n" +
                           $"Summary: Received perspectives from {message.Length} AI experts.";

        return ValueTask.FromResult(aggregatedResult);
    }
}
