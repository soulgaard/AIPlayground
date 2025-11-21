using Chatbot.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chatbot.Forms
{
  public partial class FormGraph : Form
  {
    private Graph.ChartCollection _chartCollection;

    public FormGraph()
    {
      InitializeComponent();
    }

    internal void Initialize(Graph.ChartCollection chartCollection)
    {
      _chartCollection = chartCollection;
    }

    private void FormGraph_Shown(object sender, EventArgs e)
    {
      memoEditText.Text = $"Series count: {_chartCollection.SeriesCollection.Count}";
      chartControl.Series.Clear();

      foreach (var chartData in _chartCollection.SeriesCollection)
      {
        var series = new DevExpress.XtraCharts.Series(chartData.SeriesName, DevExpress.XtraCharts.ViewType.Bar);

        // Pick chart type dynamically per series
        switch (chartData.ChartType)
        {
          case ChartType.Line:
            series.View = new DevExpress.XtraCharts.LineSeriesView();
            break;
          case ChartType.Bar:
          case ChartType.StackedBar:
            series.View = new DevExpress.XtraCharts.StackedBarSeriesView();
            break;
          case ChartType.Pie:
            series.View = new DevExpress.XtraCharts.PieSeriesView();
            break;
        }

        foreach (var point in chartData.Values)
        {
          series.Points.Add(new DevExpress.XtraCharts.SeriesPoint(point.X, point.Y));
        }

        chartControl.Series.Add(series);
      }
    }

  }
}
