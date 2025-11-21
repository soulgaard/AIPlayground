
using Azure.AI.OpenAI;
using FirstAgent;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Shared;
using System.ClientModel;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

const string endpoint = "https://ps-ai-test-resource.openai.azure.com/";
const string model = "gpt-5-mini";
const string apiKey = "<<key here>>";

string ConversationPath = Path.Combine(Path.GetTempPath(), "conversation.json");

var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
var tools = new Tools();
var methods = typeof(Tools).GetMethods(BindingFlags.Public | BindingFlags.Instance);
var toolList = methods.Select(l => AIFunctionFactory.Create(l, tools)).Cast<AITool>().ToList();


var agent = client
  .GetChatClient(model)
  .CreateAIAgent(
  instructions: "You are a time expert",
  tools: toolList
  )
  .AsBuilder()
  .Use(FunctionCallMiddleware)
  .Build();

var thread = agent.GetNewThread();

var fin = false;
while(!fin)
{
  Console.Write("> ");
  var input = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "bye")
  {
    // Serialize chat
    JsonElement serializedThread = thread.Serialize();
    await File.WriteAllTextAsync(ConversationPath, JsonSerializer.Serialize(serializedThread));

    Console.WriteLine("Chat written to: " + ConversationPath);

    fin = true;
  }
  else
  {
    var message = new ChatMessage(ChatRole.User, input);

    await foreach(var upd in agent.RunStreamingAsync(input, thread))
      Console.Write(upd);

    Console.WriteLine();
    Console.WriteLine(string.Empty.PadLeft(50, '*'));
    Console.WriteLine();
  }
}

async ValueTask<object?> FunctionCallMiddleware(
  AIAgent callingAgent, 
  FunctionInvocationContext context, 
  Func<FunctionInvocationContext, CancellationToken, 
  ValueTask<object?>> next, 
  CancellationToken cancellationToken)
{
  StringBuilder functionCallDetails = new();
  functionCallDetails.Append($"- Tool Call: '{context.Function.Name}'");
  if (context.Arguments.Count > 0)
  {
    functionCallDetails.Append($" (Args: {string.Join(",", context.Arguments.Select(x => $"[{x.Key} = {x.Value}]"))}");
  }

  Utils.WriteLineDarkGray(functionCallDetails.ToString());

  return await next(context, cancellationToken);
}

/*

//
// run single query
// 

//var question = "How to make soup?";
var question = "What is the capital of France";

var resp = await agent.RunAsync(question);
//var agentResponse = await agent.RunAsync("Tell me a short story - max 2 lines");
Console.WriteLine(resp);

Console.WriteLine("Input tokens: " + resp.Usage?.InputTokenCount);
Console.WriteLine($"Output tokens: {resp.Usage?.OutputTokenCount} " +
  $"({resp.Usage?.GetReasonTokenCount()})");

Console.WriteLine("");
Console.WriteLine("----------------------------------------");
Console.WriteLine("");

var updates = new List<AgentRunResponseUpdate>();
await foreach(var update in agent.RunStreamingAsync(question))
{
  Console.Write(update);
  updates.Add(update);
}

resp = updates.ToAgentRunResponse();
Console.WriteLine("Input tokens: " + resp.Usage?.InputTokenCount);
Console.WriteLine($"Output tokens: {resp.Usage?.OutputTokenCount} " +
  $"({resp.Usage?.GetReasonTokenCount()})");
*/
