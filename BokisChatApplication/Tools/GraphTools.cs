using Chatbot.Graph;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chatbot.Tools
{
  public class GraphTools
  {
    public string ShowGraph(ChartCollection chartData)
    {
      string jsonData = JsonConvert.SerializeObject(chartData);
      return jsonData;
    }
  }
}
