using Azure.AI.OpenAI;
using Chatbot.Agents;
using Chatbot.API;
using Chatbot.Tools;
using Microsoft.Extensions.Configuration;
using Shared;
using System.ClientModel;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

string apiKey = config["AzureOpenAI:ApiKey"];
string endpoint = config["AzureOpenAI:Endpoint"];
string model = config["AzureOpenAI:Model"];
string embeddingModel = config["AzureOpenAI:EmbeddingModel"];

Utils.WriteLineDarkGray("Getting data");

// local api
var api = new BokisAPI();

// Create client with tools
var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
var toolHelper = new ToolHelper(api, client, embeddingModel);
var toolList = await toolHelper.CreateToolListAsync();

// Example questions:
// vis den samlede fakturering for q1 2024 fordelt på de ti services med flest udgifter
// vis antal transaktioner i forhold til antal kort fordelt på kvartaler
// vis en graf, der illustrerer dette

//var agent = new ChatbotAgent(client, model, toolList);
//await agent.RunAgentAsync();

// Agent capable of creating and showing devextreme graphs to user.
var agent = new GraphAgent(client, model, toolList);
await agent.RunAgentAsync();


