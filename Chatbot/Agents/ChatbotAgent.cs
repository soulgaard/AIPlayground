using Azure.AI.OpenAI;
using Chatbot.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Shared.Extensions;

namespace Chatbot.Agents
{
  public class ChatbotAgent
  {
    string _instructions = "Du er ekspert i at analysere en række fakturaer fra BOKIS. " +
  "Du har en liste af produkter der indgår i fakturaerne. " +
  "Ved at lægge varenumre \"21444\", \"21445\", \"143347\" sammen findes kort " +
  "Ved at lægge varenumre \"20596\", \"166056\", \"143365\" sammen findes antal transaktioner" +
  "Du har data for 2024";
    
    private AzureOpenAIClient _client;
    private string? _model;
    private List<AITool> _toolList;

    public ChatbotAgent(AzureOpenAIClient client, string? model, List<AITool> toolList)
    {
      _client = client;
      _model = model;
      _toolList = toolList;
    }

    internal async Task RunAgentAsync()
    {
      AIAgent agentWithTools = _client
          .GetChatClient(_model)
          .CreateAIAgent(
              instructions: _instructions,
              tools: _toolList
              )
          .AsBuilder()
          .Use(Middleware.FunctionCallMiddleware)
          .Build();

      var thread = agentWithTools.GetNewThread();

      while (true)
      {
        Console.WriteLine("-----------------------");
        Console.WriteLine("Stil et spørgsmål");
        var question = Console.ReadLine();

        AgentRunResponse response3 = await agentWithTools.RunAsync(question, thread);
        Console.WriteLine(response3);
        response3.Usage.OutputAsInformation();
      }
    }
  }
}
