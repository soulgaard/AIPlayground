using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using Chatbot.Tools;
using Chatbot.API;

var builder = WebApplication.CreateBuilder(args);

// Load config
var config = builder.Configuration;
string apiKey = config["AzureOpenAI:ApiKey"];
string endpoint = config["AzureOpenAI:Endpoint"];
string model = config["AzureOpenAI:Model"];
string embeddingModel = config["AzureOpenAI:EmbeddingModel"];
string instructions = "Du er ekspert i at fremstille analysere fakturaer fra BOKIS.";


// Create client
var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));

var api = new BokisAPI();
var toolHelper = new ToolHelper(api, client, embeddingModel);
var toolList = await toolHelper.CreateToolListAsync();

// Create agent
AIAgent agentWithTools = client
    .GetChatClient(model)
    .CreateAIAgent(
  instructions: instructions,
  tools: toolList)
    .AsBuilder()
    .Use(Middleware.FunctionCallMiddleware)
    .Build();

// Register agent + thread
builder.Services.AddSingleton(agentWithTools);
builder.Services.AddSingleton(agentWithTools.GetNewThread());  // returns AgentThread

// Add CORS policy
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowVueDevClient",
      policy =>
      {
        policy.WithOrigins("http://localhost:5173") // Vue dev server
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
});

// Add controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable CORS before MapControllers
app.UseCors("AllowVueDevClient");

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Starting Bokis ChatBot API...");
app.Run();
