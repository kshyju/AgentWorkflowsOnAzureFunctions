// This sample demonstrates sub-workflows (nested workflows) hosted on Azure Functions.
//
// A sub-workflow is a complete workflow embedded as an executor inside a parent workflow.
// The parent workflow: Prefix -> [SubWorkflow: Uppercase -> Reverse -> AppendSuffix] -> PostProcess
//
// For input "hello", the workflow produces: "[FINAL] OLLEH [PROCESSED] [END]"
// When running on the durable runtime, the sub-workflow executes as a sub-orchestration.

using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using NestedWorkflowsFunctions;

// Step 1: Build a text processing sub-workflow
UppercaseExecutor uppercase = new();
ReverseExecutor reverse = new();
AppendSuffixExecutor append = new(" [PROCESSED]");

var subWorkflow = new WorkflowBuilder(uppercase)
    .AddEdge(uppercase, reverse)
    .AddEdge(reverse, append)
    .WithOutputFrom(append)
    .Build();

// Step 2: Bind the sub-workflow as an executor for use in the parent workflow
ExecutorBinding subWorkflowExecutor = subWorkflow.BindAsExecutor("TextProcessing");

// Step 3: Build the parent workflow that uses the sub-workflow
PrefixExecutor prefix = new("INPUT: ");
PostProcessExecutor postProcess = new();

var mainWorkflow = new WorkflowBuilder(prefix)
    .WithName("TextPipeline")
    .WithDescription("Text processing pipeline with a nested sub-workflow")
    .AddEdge(prefix, subWorkflowExecutor)
    .AddEdge(subWorkflowExecutor, postProcess)
    .WithOutputFrom(postProcess)
    .Build();

// Host on Azure Functions
using IHost app = FunctionsApplication
    .CreateBuilder(args)
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableWorkflows(workflows => workflows.AddWorkflow(mainWorkflow))
    .Build();

app.Run();
