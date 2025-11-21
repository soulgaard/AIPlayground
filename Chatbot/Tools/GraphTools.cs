using Chatbot.Forms;
using Chatbot.Graph;

namespace Chatbot.Tools
{
  public class GraphTools
  {
    // Existing WinForms method
    public string ShowGraph(ChartCollection chartData)
    {
      var thread = new Thread(() =>
      {
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        var form = new FormGraph();
        form.Initialize(chartData);

        System.Windows.Forms.Application.Run(form);
      });

      thread.SetApartmentState(ApartmentState.STA);
      thread.IsBackground = true;
      thread.Start();

      return chartData.ToString();
    }
    /*
    // NEW: Browser method
    public string ShowGraphBrowser(ChartCollection chartData, int port = 5000)
    {
      string jsonData = JsonConvert.SerializeObject(chartData);

      Task.Run(() =>
      {
        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        Console.WriteLine($"Vue graph available at http://localhost:{port}/");

        while (true)
        {
          var context = listener.GetContext();
          var response = context.Response;

          string path = context.Request.Url.AbsolutePath;

          if (path == "/data")
          {
            byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
            response.ContentType = "application/json";
            response.OutputStream.Write(buffer, 0, buffer.Length);
          }
          else
          {
            // Serve a simple Vue + Chart.js app
            string html = @"
<!DOCTYPE html>
<html>
<head>
  <title>Graph Viewer</title>
  <script src='https://cdn.jsdelivr.net/npm/vue@2'></script>
  <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
</head>
<body>
  <div id='app'>
    <canvas id='chart'></canvas>
  </div>

  <script>
    new Vue({
      el: '#app',
      mounted() {
        fetch('/data')
          .then(res => res.json())
          .then(chartCollection => {
            chartCollection.seriesCollection.forEach(series => {
              const ctx = document.getElementById('chart').getContext('2d');
              new Chart(ctx, {
                type: this.mapType(series.chartType),
                data: {
                  labels: series.values.map(v => v.x),
                  datasets: [{
                    label: series.seriesName,
                    data: series.values.map(v => v.y)
                  }]
                }
              });
            });
          });
      },
      methods: {
        mapType(type) {
          switch(type) {
            case 0: return 'line';
            case 1: return 'bar';
            case 2: return 'bar'; // stacked handled separately
            case 3: return 'pie';
            default: return 'bar';
          }
        }
      }
    });
  </script>
</body>
</html>";
            byte[] buffer = Encoding.UTF8.GetBytes(html);
            response.ContentType = "text/html";
            response.OutputStream.Write(buffer, 0, buffer.Length);
          }

          response.OutputStream.Close();
        }
      });

      return $"Graph available at http://localhost:{port}/";
    }
    */
  }
}
