# Sequential Workflow Sample

This sample demonstrates a simple three-step sequential pipeline hosted on Azure Functions using the Microsoft Agent Framework.

## Workflow: CancelOrder

| Executor | Input | Output | Description |
|----------|-------|--------|-------------|
| **OrderLookup** | `string` (order ID) | `Order` | Looks up an order by ID |
| **OrderCancel** | `Order` | `Order` | Marks the order as cancelled |
| **SendEmail** | `Order` | `string` | Sends a cancellation confirmation email |

```
OrderLookup -> OrderCancel -> SendEmail
```

## Environment Setup

See the [README.md](../README.MD) file in the parent directory for complete setup instructions, including:

- Prerequisites installation
- Durable Task Scheduler setup
- Storage emulator configuration

## Running the Sample

1. **Start the Function App**:

   ```bash
   cd SequentialWorkflow
   func start
   ```

2. **Invoke the workflow**:

   ```bash
   curl -X POST http://localhost:7071/api/workflows/CancelOrder/run \
     -H "Content-Type: text/plain" \
     -d "12345"
   ```

   Expected result: A confirmation message that a cancellation email was sent for the given order ID.
