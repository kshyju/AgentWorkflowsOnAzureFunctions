# Agents and Workflows Together Sample

This sample demonstrates how to register both standalone AI agents and workflows in a single Azure Functions app using `ConfigureDurableOptions`. It also shows two additional patterns: **reusing the same executor across multiple workflows** and **using an AI agent as a workflow step**.

## Key Concepts Demonstrated

- **`ConfigureDurableOptions`**: The more general API (vs. `ConfigureDurableWorkflows`) that gives access to both `options.Agents` and `options.Workflows`
- **Executor reuse**: The `TranslateText` executor is shared by both the Translate and TranslateAndReverse workflows
- **AI agent as workflow step**: The `Summarize` workflow uses an `AIAgent` directly as its executor
- **MCP tool exposure**: All workflows and the standalone agent are exposed as MCP tools

## Standalone Agent: Assistant

| Name | Description |
|------|-------------|
| **Assistant** | A general-purpose helpful assistant, exposed as both an HTTP endpoint and an MCP tool |

## Workflow 1: Translate

| Executor | Input | Output | Description |
|----------|-------|--------|-------------|
| **TranslateText** | `string` | `TranslationResult` | Converts input text to uppercase |
| **FormatOutput** | `TranslationResult` | `string` | Formats the result into a readable string |

## Workflow 2: TranslateAndReverse

| Executor | Input | Output | Description |
|----------|-------|--------|-------------|
| **TranslateText** | `string` | `TranslationResult` | Reuses the same TranslateText executor |
| **ReverseText** | `TranslationResult` | `string` | Reverses the translated text |

## Workflow 3: Summarize

| Executor | Input | Output | Description |
|----------|-------|--------|-------------|
| **Assistant** (AI Agent) | `string` | `string` | Uses the AI agent to summarize the input text |

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
   cd AgentsAndWorkflows
   func start
   ```

3. **Chat with the standalone Assistant agent**:

   ```bash
   curl -X POST http://localhost:7071/api/agents/Assistant/run \
     -H "Content-Type: text/plain" \
     -d "What is the speed of light?"
   ```

4. **Run the Translate workflow**:

   ```bash
   curl -X POST http://localhost:7071/api/workflows/Translate/run \
     -H "Content-Type: text/plain" \
     -d "hello world"
   ```

   Expected result: `Original: hello world => Translated: HELLO WORLD`

5. **Run the TranslateAndReverse workflow** (reuses TranslateText):

   ```bash
   curl -X POST http://localhost:7071/api/workflows/TranslateAndReverse/run \
     -H "Content-Type: text/plain" \
     -d "hello world"
   ```

   Expected result: `DLROW OLLEH`

6. **Run the Summarize workflow** (uses the AI agent as a step):

   ```bash
   curl -X POST http://localhost:7071/api/workflows/Summarize/run \
     -H "Content-Type: text/plain" \
     -d "The speed of light in vacuum is approximately 299,792,458 meters per second..."
   ```

   Expected result: An AI-generated summary of the input text.
