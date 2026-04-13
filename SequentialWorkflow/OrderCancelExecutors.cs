using Microsoft.Agents.AI.Workflows;

namespace SequentialWorkflowFunctions;

internal sealed class OrderLookup() : Executor<string, Order>("OrderLookup")
{
    public override ValueTask<Order> HandleAsync(
        string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        // Simulate looking up an order by ID
        return ValueTask.FromResult(new Order(
            Id: message,
            OrderDate: DateTime.UtcNow.AddDays(-1),
            IsCancelled: false,
            Customer: new Customer(Name: "Jerry", Email: "jerry@example.com")));
    }
}

internal sealed class OrderCancel() : Executor<Order, Order>("OrderCancel")
{
    public override ValueTask<Order> HandleAsync(
        Order message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        // Cancel the order
        return ValueTask.FromResult(message with { IsCancelled = true });
    }
}

internal sealed class SendEmail() : Executor<Order, string>("SendEmail")
{
    public override ValueTask<string> HandleAsync(
        Order message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(
            $"Cancellation email sent for order {message.Id} to {message.Customer.Email}.");
    }
}

internal sealed record Order(string Id, DateTime OrderDate, bool IsCancelled, Customer Customer);
internal sealed record Customer(string Name, string Email);
