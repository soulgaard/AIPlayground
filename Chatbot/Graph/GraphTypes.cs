namespace Chatbot.Graph
{
  public class ChartPoint
  {
    public string X { get; set; } = string.Empty;
    public double Y { get; set; }
  }

  public enum ChartType
  {
    Line,
    Bar,
    StackedBar,
    Pie
  }

  /// <summary>
  /// Class defining a single chart series.
  /// </summary>
  public class ChartData
  {
    public ChartType ChartType { get; set; }
    public string SeriesName { get; set; } = string.Empty;
    public List<ChartPoint> Values { get; set; } = new();

    /// <summary>
    /// Print graph data as a string for debugging.
    /// </summary>
    /// <returns></returns>
    override public string ToString()
    {
      var valuesStr = string.Join(", ", Values.Select(v => $"{v.X}:{v.Y}"));
      return $"Chart Type: {ChartType}{Environment.NewLine}" +
             $"Series: {SeriesName}{Environment.NewLine}" +
             $"Values: {valuesStr}";
    }
  }

  /// <summary>
  /// Container for multiple graph series.
  /// </summary>
  public class ChartCollection
  {
    public List<ChartData> SeriesCollection { get; set; } = new();

    /// <summary>
    /// Print series collection as a string for debugging.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      var seriesStr = string.Join($"{Environment.NewLine}---{Environment.NewLine}", SeriesCollection.Select(s => s.ToString()));
      return $"Chart Collection:{Environment.NewLine}{seriesStr}";
    }
  }

}
