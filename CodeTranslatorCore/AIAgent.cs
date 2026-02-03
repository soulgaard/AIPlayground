using Azure.AI.OpenAI;
using CodeTranslatorCore;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeTranslator
{
  internal class AIAgent
  {
    private Microsoft.Agents.AI.AIAgent _agent;
    private AgentThread _thread;

    public AIAgent(string sourceLanguage, string destLanguage)
    {
      var client = new AzureOpenAIClient(new Uri(Program._endpoint), new ApiKeyCredential(Program._apiKey));
      //var tools = new Tools();
      //var methods = typeof(Tools).GetMethods(BindingFlags.Public | BindingFlags.Instance);
      //var toolList = methods.Select(l => AIFunctionFactory.Create(l, tools)).Cast<AITool>().ToList();

      var instructions = @$"You are a translator.
The following code should have all usertext translated from {sourceLanguage} to {destLanguage}.
Code can be c#, vue or javascript etc.
Do not tranlate between da-DK and en-US in code syntax, only in user visible text such as variable names, comments, strings etc.
Only return code";


      _agent = client
        .GetChatClient(Program._model)
        .CreateAIAgent(
        instructions: instructions
        //tools: toolList
        )
        .AsBuilder()
        //.Use(FunctionCallMiddleware)
        .Build();

      _thread = _agent.GetNewThread();

    }

    public void ResetThread()
    {
      _thread = _agent.GetNewThread();
    } 

    public async Task<string> TranslateCodeAsync(string codeSnippet)
    {
      Console.WriteLine("Translating");

      var result = await _agent.RunAsync(codeSnippet, _thread);
      Console.WriteLine("Translated");
      Console.WriteLine("Result");
      Console.WriteLine();
      Console.WriteLine(result.ToString());
      return result.ToString();
    }
  }
}
