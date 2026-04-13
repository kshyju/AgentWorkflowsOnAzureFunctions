// This sample demonstrates how to register both standalone AI agents and workflows
// in a single Azure Functions app using ConfigureDurableOptions. The AI agent is
// exposed as both an HTTP endpoint and an MCP tool, while the workflow is exposed
// only as an MCP tool.

using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using AgentsAndWorkflowsFunctions;

string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
string deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT")
    ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT is not set.");
string? azureOpenAiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

// Create Azure OpenAI client
AzureOpenAIClient openAiClient = !string.IsNullOrEmpty(azureOpenAiKey)
    ? new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(azureOpenAiKey))
    : new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());
ChatClient chatClient = openAiClient.GetChatClient(deploymentName);

// Define a standalone AI agent
AIAgent assistant = chatClient.AsAIAgent(
    "You are a helpful assistant. Answer questions clearly and concisely.",
    "Assistant",
    description: "A general-purpose helpful assistant.");

// Define workflow executors
TranslateText translateText = new();
FormatOutput formatOutput = new();

// Build the Translate workflow: TranslateText -> FormatOutput
Workflow translateWorkflow = new WorkflowBuilder(translateText)
    .WithName("Translate")
    .WithDescription("Translate text to uppercase and format the result")
    .AddEdge(translateText, formatOutput)
    .Build();

// Register both the AI agent and the workflow using ConfigureDurableOptions
using IHost app = FunctionsApplication
    .CreateBuilder(args)
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableOptions(options =>
    {
        options.Agents.AddAIAgent(assistant,
            enableHttpTrigger: true, enableMcpToolTrigger: true);
        options.Workflows.AddWorkflow(translateWorkflow,
            exposeStatusEndpoint: false, exposeMcpToolTrigger: true);
    })
    .Build();

app.Run();
