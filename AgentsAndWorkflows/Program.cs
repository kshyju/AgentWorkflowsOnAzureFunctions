// This sample demonstrates:
// 1. Registering both standalone AI agents and workflows in a single Functions app
// 2. Reusing the same executor (TranslateText) across multiple workflows
// 3. Using an AI agent as an executor step inside a workflow

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

// Define a standalone AI agent (also used as an executor in the Summarize workflow)
AIAgent assistant = chatClient.AsAIAgent(
    "You are a helpful assistant. Answer questions clearly and concisely.",
    "Assistant",
    description: "A general-purpose helpful assistant.");

// Define workflow executors
TranslateText translateText = new();
FormatOutput formatOutput = new();
ReverseText reverseText = new();

// Workflow 1: Translate — TranslateText -> FormatOutput
Workflow translateWorkflow = new WorkflowBuilder(translateText)
    .WithName("Translate")
    .WithDescription("Translate text to uppercase and format the result")
    .AddEdge(translateText, formatOutput)
    .Build();

// Workflow 2: TranslateAndReverse — reuses TranslateText, then reverses the result
Workflow translateAndReverseWorkflow = new WorkflowBuilder(translateText)
    .WithName("TranslateAndReverse")
    .WithDescription("Translate text to uppercase and reverse it")
    .AddEdge(translateText, reverseText)
    .Build();

// Workflow 3: Summarize — uses the AI agent as an executor step
Workflow summarizeWorkflow = new WorkflowBuilder(assistant)
    .WithName("Summarize")
    .WithDescription("Summarize the given text using an AI assistant")
    .Build();

// Register both the standalone AI agent and all workflows
using IHost app = FunctionsApplication
    .CreateBuilder(args)
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableOptions(options =>
    {
        options.Agents.AddAIAgent(assistant,
            enableHttpTrigger: true, enableMcpToolTrigger: true);

        options.Workflows.AddWorkflow(translateWorkflow,
            exposeStatusEndpoint: false, exposeMcpToolTrigger: true);
        options.Workflows.AddWorkflow(translateAndReverseWorkflow,
            exposeStatusEndpoint: false, exposeMcpToolTrigger: true);
        options.Workflows.AddWorkflow(summarizeWorkflow,
            exposeStatusEndpoint: false, exposeMcpToolTrigger: true);
    })
    .Build();

app.Run();
