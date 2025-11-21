using Azure.Core;
using BokisChatApi.Dtos;
using Chatbot.Graph;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Newtonsoft.Json;
using System.Threading;

namespace BokisChatApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ChatController : ControllerBase
  {
    private readonly AIAgent _agent;
    private readonly AgentThread _thread;

    public ChatController(AIAgent agent, AgentThread thread)
    {
      _agent = agent;
      _thread = thread;
    }

    /// <summary>
    /// Ask a question to the Bokis ChatBot.
    /// </summary>
    [HttpPost("ask")]
    public async Task<ActionResult<BokisChatResponse>> Ask([FromBody] ChatRequest request)
    {
      var response = await _agent.RunAsync(request.Question, _thread);
      return Ok(new BokisChatResponse
      {
        Question = request.Question,
        Answer = response.ToString()
      });
    }

    [HttpGet("graph")]
    public async Task<ActionResult<ChartCollection>> GetGraphReal()
    {
      var result = await _agent.RunAsync(
        "call tool 'get_graph' and return the " +
        "resulting json with no extra text", _thread);

      var messages = result.Messages;

      var json = messages
          .Where(m => m.Role == ChatRole.Assistant)
          .SelectMany(m => m.Contents)
          //.Where(c => c.GetType() = == "text")
          //.Select(c => c.)
          .Last();   // <-- this is your JSON string

      // Now parse it
      var parsed = JsonConvert.DeserializeObject<ChartCollection>(result.Text);

      return Ok(parsed);
    }


    [HttpGet("graphtest")]
    public ActionResult<ChartCollection> GetGraph()
    {

      var collection = new ChartCollection
      {
        SeriesCollection = new List<ChartData>
        {
            new ChartData
            {
                ChartType = ChartType.Bar,
                SeriesName = "Salg",
                Values = new List<ChartPoint>
                {
                    new ChartPoint { X = "Jan", Y = 42 },
                    new ChartPoint { X = "Feb", Y = 27 },
                    new ChartPoint { X = "Mar", Y = 35 }
                }
            },
            new ChartData
            {
                ChartType = ChartType.Line,
                SeriesName = "Omsætning",
                Values = new List<ChartPoint>
                {
                    new ChartPoint { X = "Jan", Y = 100 },
                    new ChartPoint { X = "Feb", Y = 120 },
                    new ChartPoint { X = "Mar", Y = 90 }
                }
            }
        }
      };

      return Ok(collection);
    }



  }
}
