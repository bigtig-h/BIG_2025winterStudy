using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;

#pragma warning disable SKEXP0001, SKEXP0010, SKEXP0020, SKEXP0050

var config = new ConfigurationBuilder()
           .AddJsonFile("appsetting.json")
           .Build();

var builder = Kernel.CreateBuilder();

builder.AddOpenAIChatCompletion(
    config["OPEN_AI_MODEL"],
    config["OPEN_AI_KEY"],
    config["OPEN_AI_ORG_ID"]);

var kernel = builder.Build();

var googleConnector = new GoogleConnector(config["GOOGLE_KEY"], config["GOOGLE_ID"]);

kernel.ImportPluginFromObject(new WebSearchEnginePlugin(googleConnector), "google");

Console.WriteLine("Search Google with Semantic Kernel! (type 'quit' to exit): ");
string query = Console.ReadLine();

while (query.ToLower() != "quit")
{
    Console.WriteLine("Searching...");

    var function = kernel.Plugins["google"]["search"];
    var result = await kernel.InvokeAsync(function, new()
    {
        ["query"] = query
    });
    Console.WriteLine($"Result: {result}\n");

    Console.WriteLine("Search Google with Semantic Kernel! (type 'quit' to exit): ");
    query = Console.ReadLine();

}

