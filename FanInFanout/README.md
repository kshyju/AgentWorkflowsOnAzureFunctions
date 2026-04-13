# Fan-Out / Fan-In (Parallel Execution) Sample

This sample demonstrates the fan-out/fan-in pattern — multiple AI agents process the same input concurrently, then an aggregator combines their responses.

## Workflow: ExpertReview

| Executor | Input | Output | Description |
|----------|-------|--------|-------------|
| **ParseQuestion** | `string` | `string` | Validates and formats the incoming question |
| **Physicist** (AI Agent) | `string` | `string` | Answers from a physics perspective |
| **Chemist** (AI Agent) | `string` | `string` | Answers from a chemistry perspective |
| **Aggregator** | `string[]` | `string` | Combines all expert responses into a summary |

```
ParseQuestion -> [Physicist, Chemist] (parallel) -> Aggregator
```

## Prerequisites

This sample requires an **Azure OpenAI** resource. Update `local.settings.json` with your endpoint, deployment name, and optionally an API key before running.

## Environment Setup

See the [README.md](../README.MD) file in the parent directory for complete setup instructions, including:

- Prerequisites installation
- Durable Task Scheduler setup
- Storage emulator configuration

## Running the Sample

1. **Configure Azure OpenAI** in `local.settings.json`:

   ```json
   {
     "AZURE_OPENAI_ENDPOINT": "<YOUR_AZURE_OPENAI_ENDPOINT>",
     "AZURE_OPENAI_DEPLOYMENT": "<YOUR_AZURE_OPENAI_DEPLOYMENT>",
     "AZURE_OPENAI_KEY": ""
   }
   ```

   If `AZURE_OPENAI_KEY` is empty, the sample uses `AzureCliCredential` for authentication.

2. **Start the Function App**:

   ```bash
   cd FanInFanout
   func start
   ```

3. **Invoke the workflow**:

   ```bash
   curl -X POST http://localhost:7071/api/workflows/ExpertReview/run \
     -H "Content-Type: text/plain" \
     -d "Why is the sky blue"
   ```

   Expected result: A combined response from both the physics and chemistry AI agents.
