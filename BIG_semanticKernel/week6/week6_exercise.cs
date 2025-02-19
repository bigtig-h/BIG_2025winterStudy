using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.MongoDB;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using MongoDB.Driver;
using week6;

#pragma warning disable SKEXP0001, SKEXP0010, SKEXP0020, SKEXP0050
public class week6_exercise
{
    static string MongoDBAtlasConnectionString = "mongodb+srv://bigtig:Jf3tvQYN2jgPPoGU@movies.5pcfz.mongodb.net/?" +
        "retryWrites=true&w=majority&appName=movies";
    static string SearchIndexName = "default";
    static string databaseName = "semantic-kernel";
    static string CollectionName = "movies";
    static MemoryBuilder memoryBuilder;

    public static async Task Main( string[] args )
    {
        var config = new ConfigurationBuilder()
           .AddJsonFile("appsetting.json")
           .Build();
        string TextEmbeddingModelName = config["TEXT_EMBEDDING_MODEL"];
        string chatModel = "gpt-4o-2024-08-06";
        string OpenAIAPIKey = config["OPEN_AI_KEY"];


        memoryBuilder = new MemoryBuilder();
        memoryBuilder.WithOpenAITextEmbeddingGeneration(TextEmbeddingModelName, OpenAIAPIKey);

        var mongoDBMemoryStore = new MongoDBMemoryStore(MongoDBAtlasConnectionString,
            databaseName, SearchIndexName);
        memoryBuilder.WithMemoryStore(mongoDBMemoryStore);
        var memory = memoryBuilder.Build();

        //await FetchAndSaveMovieDocuments(memory, 1000);

        Console.WriteLine("Welcome to the Movie Recommendation System!");
        Console.WriteLine("Type 'x' and press Enter to exit.");
        Console.WriteLine("============================================");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("Tell me what sort of film you want to watch..");
            Console.WriteLine();

            Console.Write("> ");

            var userInput = Console.ReadLine();

            if (userInput.ToLower() == "x")
            {
                Console.WriteLine("Exiting application..");
                break;
            }

            Console.WriteLine();

            var memories = memory.SearchAsync(CollectionName, userInput, limit: 3, minRelevanceScore: 0.6);

            Console.WriteLine(String.Format("{0,-20} {1,-50} {2,-10} {3,-15}", "Title", "Plot", "Year", "Relevance (0 - 1)"));
            Console.WriteLine(new String('-', 95)); // Adjust the length based on your column widths

            await foreach (var mem in memories)
            {
                Console.WriteLine(String.Format("{0,-20} {1,-50} {2,-10} {3,-15}",
                    mem.Metadata.Id,
                    mem.Metadata.Description.Length > 47 ? mem.Metadata.Description.Substring(0, 47) + "..." : mem.Metadata.Description, // Truncate long descriptions
                    mem.Metadata.AdditionalMetadata,
                    mem.Relevance.ToString("0.00"))); // Format relevance score to two decimal places
            }
        }




    }


    private static async Task FetchAndSaveMovieDocuments( ISemanticTextMemory memory, int limitSize )
    {
        MongoClient mongoClient = new MongoClient(MongoDBAtlasConnectionString);
        var movieDB = mongoClient.GetDatabase("sample_mflix");
        var movieCollection = movieDB.GetCollection<Movie>("movies");
        List<Movie> movieDocuments;

        Console.WriteLine("Fetching documents from MongoDB...");

        movieDocuments = movieCollection.Find(m => true).Limit(limitSize).ToList();

        movieDocuments.ForEach(movie =>
        {
            if (movie.Plot == null)
            {
                movie.Plot = "UNKNOWN";
            }
        });

        foreach (var movie in movieDocuments)
        {
            try
            {
                await memory.SaveReferenceAsync(
                collection: CollectionName,
                description: movie.Plot,
                text: movie.Plot,
                externalId: movie.Title,
                externalSourceName: "Sample_Mflix_Movies",
                additionalMetadata: movie.Year.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
    }




}







