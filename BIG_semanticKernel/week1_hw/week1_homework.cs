using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

// 루프 형식으로 사용자가 특정 문구를 입력해야 종료되는 챗봇 생성
// Local LLM 을 활용하여 chatCompletion을 생성하도록!

var modelId = "llama3.2";
var endpoint = new Uri("http://127.0.0.1:11434/v1");

# pragma warning disable SKEXP0010
var builder = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId: modelId, apiKey: null, endpoint: endpoint);

var kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddSystemMessage("You are a helpful assistant.");

while (true)
{
    Console.Write("You: ");
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage))
    {
        break;
    }

    history.AddUserMessage(userMessage);

    var response = await chatService.GetChatMessageContentAsync(history);

    Console.WriteLine($"Bot: {response.Content}");

    history.AddMessage(response.Role, response.Content ?? string.Empty);
}
