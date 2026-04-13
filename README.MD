# Durable AI Agent Workflows on Azure Functions

Sample projects demonstrating how to host [Microsoft Agent Framework](https://github.com/microsoft/agent-framework) workflows on Azure Functions. Each sample is a standalone Azure Functions app you can run locally.

## Samples

| Sample | Description |
|---|---|
| [SequentialWorkflow](./SequentialWorkflow) | A simple three-step pipeline: look up an order, cancel it, and send a confirmation email. |
| [FanInFanout](./FanInFanout) | Parallel execution — a physics expert and chemistry expert answer the same question concurrently, then an aggregator combines their responses. Requires Azure OpenAI. |
| [HumanInTheLoop](./HumanInTheLoop) | Expense reimbursement with three approval gates — manager approval followed by parallel budget and compliance approvals using `RequestPort`. |
| [WorkflowMcpTool](./WorkflowMcpTool) | Expose durable workflows as [MCP](https://modelcontextprotocol.io/) tools — two workflows (Translate and OrderLookup) are callable via the MCP Inspector or any MCP-compatible client. |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for running the Durable Task Scheduler emulator)
- Azure OpenAI resource (only for the FanInFanout sample)

## Setting Up the Durable Task Scheduler Emulator

The samples use the [Durable Task Scheduler](https://learn.microsoft.com/azure/azure-functions/durable/durable-task-scheduler/choose-orchestration-framework) for durable workflow state. For local development, run the emulator using Docker:

```bash
docker run -d --name dts-emulator -p 8080:8080 -p 8082:8082 mcr.microsoft.com/dts/dts-emulator:latest
```

This starts the emulator on port `8080` (gRPC) and `8082` (dashboard). The samples are preconfigured to connect to this emulator via the `DURABLE_TASK_SCHEDULER_CONNECTION_STRING` setting in each `local.settings.json`:

```
Endpoint=http://localhost:8080;TaskHub=default;Authentication=None
```

You also need Azurite running for Azure Storage emulation (required by the Azure Functions runtime). See the section below.

## Setting Up Azurite (Azure Storage Emulator)

[Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite) is an open-source Azure Storage emulator required by the Azure Functions host. The samples reference it via `"AzureWebJobsStorage": "UseDevelopmentStorage=true"` in `local.settings.json`.

**Option 1: Docker (recommended)**

```bash
docker run -d --name azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

This starts Blob (10000), Queue (10001), and Table (10002) services.

**Option 2: npm**

```bash
npm install -g azurite
azurite --silent
```

**Option 3: VS Code Extension**

Install the [Azurite extension](https://marketplace.visualstudio.com/items?itemName=Azurite.azurite) and start it from the command palette (`Azurite: Start`).

## Running Locally

1. Start the Durable Task Scheduler emulator and Azurite.

2. Navigate to a sample folder and run:

   ```bash
   cd SequentialWorkflow
   func start
   ```

3. Invoke the workflow:

   ```bash
   curl -X POST http://localhost:7071/api/workflows/CancelOrder/run \
     -H "Content-Type: text/plain" \
     -d "12345"
   ```

For the **FanInFanout** sample, update `local.settings.json` with your Azure OpenAI endpoint, deployment name, and optionally an API key before running.

For the **HumanInTheLoop** sample, the workflow pauses at approval gates. Use the status and respond endpoints to interact:

```bash
# Check status
curl http://localhost:7071/api/workflows/ExpenseReimbursement/status/{runId}

# Submit approval
curl -X POST http://localhost:7071/api/workflows/ExpenseReimbursement/respond/{runId} \
  -H "Content-Type: application/json" \
  -d '{"eventName": "ManagerApproval", "response": {"approved": true, "comments": "Looks good"}}'
```

## Blog Post

These samples accompany the blog post: [Durable AI Agent Workflows on Azure Functions](https://techiesweb.net/2026/04/12/hosting-durable-ai-agent-workflows-on-azure-functions.html)
