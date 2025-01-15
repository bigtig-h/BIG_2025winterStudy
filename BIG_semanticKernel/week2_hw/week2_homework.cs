
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.RegularExpressions;
using ImageContent = Microsoft.SemanticKernel.ImageContent;
using TextContent = Microsoft.SemanticKernel.TextContent;

var endpoint = new Uri("http://localhost:11434/v1");
var modelId = "llava";
# pragma warning disable SKEXP0010
var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId: modelId, endpoint: endpoint, apiKey: null);

var kernel = builder.Build();


var chatService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddSystemMessage("You are help full assistant that will help you with your questions.");

byte[] imageBytes = null; // 이미지 데이터를 저장할 변수
string imagePathPattern = @"[a-zA-Z]:\\[^:*?""<>|\r\n]+(?:\.jpg|\.jpeg|\.png|\.bmp|\.gif)"; // 이미지 경로를 탐지하는 정규식

while (true)
{
    Console.Write("You: ");
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage))
    {
        break;
    }

    // 이미지 경로 탐지
    var match = Regex.Match(userMessage, imagePathPattern);
    if (match.Success)
    {
        var imagePath = match.Value;
        try
        {
            imageBytes = File.ReadAllBytes(imagePath); // 이미지 로드
            history.AddUserMessage($"이미지 경로를 탐지하였습니다: {imagePath}");
            history.AddUserMessage("이미지를 성공적으로 불러왔습니다.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"이미지를 불러오는 중 오류가 발생했습니다: {ex.Message}");
            continue;
        }
    }
    else if (imageBytes != null) // 이미지가 이미 로드된 상태에서 사용자 입력 처리
    {
        history.AddUserMessage(userMessage);

        // 텍스트와 이미지를 고려하도록 요청 생성
        history.AddUserMessage(
        [
            new TextContent(userMessage),
            new ImageContent(imageBytes, "image/jpeg")
        ]);

        var response = await chatService.GetChatMessageContentAsync(history);

        Console.WriteLine($"Bot: {response.Content}");

        history.AddMessage(response.Role, response.Content ?? string.Empty);
    }
    else
    {
        // 이미지 없이 일반 입력 처리
        history.AddUserMessage(userMessage);

        var response = await chatService.GetChatMessageContentAsync(history);

        Console.WriteLine($"Bot: {response.Content}");

        history.AddMessage(response.Role, response.Content ?? string.Empty);
    }
}