using Azure.AI.OpenAI;
using Chatbot.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.Agents
{
  public class GraphAgent
  {
    string _instructions =
      "Du er ekspert i at fremstille analysere fakturaer fra BOKIS, der er et fælles indkøbs- og udviklingsselskab " +
      "på betalingsområdet etableret af Lokale Pengeinstitutter og Landsdækkende Banker​​.. " +
      "Hvis brugeren specifikt beder om det, kan du bruge værktøjet 'graph_tool' til at vise en graf. " +
      "Ved at lægge varenumre \"21444\", \"21445\", \"143347\" sammen findes kort " +
      "Ved at lægge varenumre \"20596\", \"166056\", \"143365\" sammen findes antal transaktioner " +
      "Du har data for 2024 ";

    private AzureOpenAIClient _client;
    private string? _model;
    private List<AITool> _toolList;

    public GraphAgent(AzureOpenAIClient client, string? model, List<AITool> toolList)
    {
      _client = client;
      _model = model;
      _toolList = toolList;
    }

    public async Task RunAgentAsync()
    {
      // Create agent with tools
      AIAgent agent = _client
      .GetChatClient(_model)
      .CreateAIAgent(
          instructions: _instructions,
          tools: _toolList
          )
      .AsBuilder()
      .Use(Middleware.FunctionCallMiddleware)
      .Build();

      // Create thread for conversation
      var thread = agent.GetNewThread();

      Console.WriteLine("Hvordan kan jeg hjælpe");

      while (true)
      {
        Console.WriteLine("-----------------------");
        var question = Console.ReadLine();

        // Handle current request
        AgentRunResponse response = await agent.RunAsync(question, thread);
        Console.WriteLine(response);
        response.Usage.OutputAsInformation();
      }
    }
  }
}
