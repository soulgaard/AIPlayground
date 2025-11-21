using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var client = new OllamaApiClient("http://localhost:11434", "llama3.2:3b");

var agent = new ChatClientAgent(client);
var resp = await agent.RunAsync("What is the capital of sweden?");
Console.WriteLine(resp);
Console.WriteLine("----------------");

await foreach(var update in agent.RunStreamingAsync("how to make a cake"))
  Console.Write(update);

