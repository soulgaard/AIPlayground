using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Shared;
using System.Text;

namespace Chatbot.Tools
{
  public class Middleware
  {
    /// <summary>
    /// Log tool calls made by the agent to the console.
    /// </summary>
    /// <param name="callingAgent"></param>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<object?> FunctionCallMiddleware(AIAgent callingAgent, FunctionInvocationContext context, Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken cancellationToken)
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
  }
}
