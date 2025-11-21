using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string apiKey = config["AzureOpenAI:ApiKey"];
string endpoint = config["AzureOpenAI:Endpoint"];
string model = config["AzureOpenAI:Model"];
string embeddingModel = config["AzureOpenAI:EmbeddingModel"];
string instructions =
  "Du er ekspert i at fremstille analysere fakturaer fra BOKIS. ";

// local api
//var api = new BokisAPI();

// Create client with tools
var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
//var toolHelper = new ToolHelper(api, client, embeddingModel);
//var toolList = await toolHelper.CreateToolListAsync();


// Create agent with tools
AIAgent agentWithTools = client
.GetChatClient(model)
.CreateAIAgent(
    instructions: instructions
    //tools: _toolList
    )
.AsBuilder()
//.Use(Middleware.FunctionCallMiddleware)
.Build();

// Create thread for conversation
var thread = agentWithTools.GetNewThread();

Console.WriteLine("Api ville spørge her");

while (true)
{
  Console.WriteLine("-----------------------");
  var question = Console.ReadLine();

  // Handle current request
  AgentRunResponse response3 = await agentWithTools.RunAsync(question, thread);
  Console.WriteLine(response3);
  //response3.Usage.OutputAsInformation();
}

