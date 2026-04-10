using Azure.AI.OpenAI;
using Azure.Core;
using CodeTranslatorCore;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
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

    public enum TranslationTypeType
    {
      /// <summary>
      /// Used for code (vue + c#)
      /// </summary>
      Code,
      /// <summary>
      /// Used for json language files
      /// </summary>
      JsonVueLanguageFiles,
      ResourceLanguageFiles
    }

    public AIAgent(string sourceLanguage, string destLanguage, TranslationTypeType translationType)
    {
      var options = new AzureOpenAIClientOptions
      {
        NetworkTimeout = TimeSpan.FromMinutes(20),
      };

      var client = new AzureOpenAIClient(
        new Uri(Program._endpoint), 
        new ApiKeyCredential(Program._apiKey),
        options);
      //var tools = new Tools();
      //var methods = typeof(Tools).GetMethods(BindingFlags.Public | BindingFlags.Instance);
      //var toolList = methods.Select(l => AIFunctionFactory.Create(l, tools)).Cast<AITool>().ToList();

      var instructions = string.Empty;

      switch (translationType)
      {
        case TranslationTypeType.Code:
          instructions = @$"You are a translator.
The following code should have all usertext translated from {sourceLanguage} to {destLanguage}.
Code can be c#, vue or javascript etc.
Do not tranlate between da-DK and en-US in code syntax, only in user visible text such as variable names, comments, strings etc.
If no user visible text is present, return the code as is.
Do not correct the words Grantet, Nemkonto, MitID, OCES or similar.
Only return code";
          break;
        case TranslationTypeType.JsonVueLanguageFiles:
          instructions = @$"You are a translator.
The following json code represents the {sourceLanguage} language.
translate the value fields from {sourceLanguage} to {destLanguage}.
Do not correct the words Grantet, Nemkonto, MitID, OCES or similar.
do not change the variable names
Only return valid json code";
          break;
        case TranslationTypeType.ResourceLanguageFiles:
          instructions = @$"You are a translator.
The following c# resx code represents the {sourceLanguage} language.
translate the value fields from {sourceLanguage} to {destLanguage}.
Do not correct the words Grantet, Nemkonto, MitID, OCES or similar.
do not change the variable names
Translate only in the content of value fields. Never change or translate in the name field.
  <data name=""Name"" xml:space=""preserve"">
    <value>text to translate</value>
  </data>
Only return valid code";
          break;
        default:
          throw new NotImplementedException();
      }


      // Translating code
      //      var instructions = @$"You are a translator.
      //The following code should have all usertext translated from {sourceLanguage} to {destLanguage}.
      //Code can be c#, vue or javascript etc.
      //Do not tranlate between da-DK and en-US in code syntax, only in user visible text such as variable names, comments, strings etc.
      //If no user visible text is present, return the code as is.
      //Do not correct the words Grantet, Nemkonto, MitID, OCES or similar.
      //Only return code";

      //Translating json language files
//      var instructions = @$"You are a translator.
//The followin json code represents the {sourceLanguage} language.
//translate the value fields from {sourceLanguage} to {destLanguage}.
//Do not correct the words Grantet, Nemkonto, MitID, OCES or similar.
//do not change the variable names
//Only return valid json code";

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
      if (string.IsNullOrWhiteSpace(codeSnippet)) return codeSnippet;
      var result = await _agent.RunAsync(codeSnippet, _thread);
      return result.ToString();
    }
  }
}
