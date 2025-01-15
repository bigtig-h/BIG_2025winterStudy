using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using week3;

//var config = new ConfigurationBuilder().
//    AddJsonFile("appsetting.json")
//    .Build();

//var builder = Kernel.CreateBuilder()
//        .AddOpenAIChatCompletion(config["OPEN_AI_MODEL"], config["OPEN_AI_KEY"], config["OPEN_AI_ORG_ID"]);
//Kernel kernel = builder.Build();

var modelId = "llama3.2";
var endpoint = new Uri("http://127.0.0.1:11434/v1");

# pragma warning disable SKEXP0010
var builder = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId: modelId, apiKey: null, endpoint: endpoint);
var kernel = builder.Build();


var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();



kernel.Plugins.AddFromType<LightsPlugin>("Lights");

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();

string? userInput;
while (true)
{
    // Add user input
    Console.Write("User > ");
    userInput = Console.ReadLine();
    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    if (userInput == "이제 그만")
    {
        break;
    }

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
}



