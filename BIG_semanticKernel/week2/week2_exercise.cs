using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

internal class week2_exercise
{

    static async Task Main( string[] args )
    {
        var config = new ConfigurationBuilder().
            AddJsonFile("appsetting.json")
            .Build();

        Console.WriteLine("Launching Semantic Kernel Sandpit");

        var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(config["OPEN_AI_MODEL"], config["OPEN_AI_KEY"], config["OPEN_AI_ORG_ID"]);


        Kernel kernel = builder.Build();
        // Create chat history
        var history = new ChatHistory();


        // Get chat completion service
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Start the conversation
        string? userInput;
        while (true)
        {
            // Add user input
            Console.Write("User > ");
            userInput = Console.ReadLine();
            history.AddUserMessage(userInput);

            // Enable auto function calling
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()

            {
            };

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

            // Get user input again
        }
    }
}