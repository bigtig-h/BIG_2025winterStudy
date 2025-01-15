using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;


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



public class LightsPlugin
{
    // Mock data for the lights
    private readonly List<LightModel> lights = new()
   {
      new LightModel { Id = 1, Name = "Table Lamp", IsOn = false, Brightness = 100, Hex = "FF0000" },
      new LightModel { Id = 2, Name = "Porch light", IsOn = false, Brightness = 50, Hex = "00FF00" },
      new LightModel { Id = 3, Name = "Chandelier", IsOn = true, Brightness = 75, Hex = "0000FF" }
   };

    [KernelFunction("get_lights")]
    [Description("Gets a list of lights and their current state")]
    [return: Description("An array of lights")]
    public async Task<List<LightModel>> GetLightsAsync( )
    {
        return lights;
    }

    [KernelFunction("get_state")]
    [Description("Gets the state of a particular light")]
    [return: Description("The state of the light")]
    public async Task<LightModel?> GetStateAsync( [Description("The ID of the light")] int id )
    {
        // Get the state of the light with the specified ID
        return lights.FirstOrDefault(light => light.Id == id);
    }

    [KernelFunction("change_state")]
    [Description("Changes the state of the light")]
    [return: Description("The updated state of the light; will return null if the light does not exist")]
    public async Task<LightModel?> ChangeStateAsync( int id, LightModel LightModel )
    {
        var light = lights.FirstOrDefault(light => light.Id == id);

        if (light == null)
        {
            return null;
        }

        // Update the light with the new state
        light.IsOn = LightModel.IsOn;
        light.Brightness = LightModel.Brightness;
        light.Hex = LightModel.Hex;

        return light;
    }
}

public class LightModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("is_on")]
    public bool? IsOn { get; set; }

    [JsonPropertyName("brightness")]
    public byte? Brightness { get; set; }

    [JsonPropertyName("hex")]
    public string? Hex { get; set; }
}


