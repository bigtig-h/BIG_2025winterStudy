using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

internal class week1_exercise
{
    static async Task Main( string[] args )
    {
        // appsetting.json 파일을 설정해야 함~
        // appsetting.json 속성 - 출력 디렉토리로 복사 - 항상 복사
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsetting.json")
            .Build();

        var builder = Kernel.CreateBuilder();

        builder.AddOpenAIChatCompletion(
            config["OPEN_AI_MODEL"],
            config["OPEN_AI_KEY"],
            config["OPEN_AI_ORG_ID"]);

        var kernel = builder.Build();

        var chat = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();

        history.AddSystemMessage("내가 보여주는 이미지를 토대로 대답해줘! 친절하게 대답해.");

        var img = "https://ichef.bbci.co.uk/ace/ws/800/cpsprodpb/7624/production/_104444203_d03fb5eb-685c-42c3-8fa2-eea0ee2dac26.jpg.webp";

        var message = new ChatMessageContentItemCollection
    {
        new TextContent("해당 사진에 대해 묘사해줘!"),
        new ImageContent(new Uri(img))
    };

        history.AddUserMessage(message);

        var result = await chat.GetChatMessageContentAsync(history);
        Console.WriteLine($"Let me describe that image for you: {result}");

    }

}