using Microsoft.Extensions.Configuration;

namespace CodeTranslatorCore
{
  internal static class Program
  {
    public static string? _apiKey;
    public static string? _endpoint;
    public static string? _model;
    public static string? _embeddingModel;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      var config = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
      .Build();

      _apiKey = config["AzureOpenAI:ApiKey"];
      _endpoint = config["AzureOpenAI:Endpoint"];
      _model = config["AzureOpenAI:Model"];
      _embeddingModel = config["AzureOpenAI:EmbeddingModel"];

      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ApplicationConfiguration.Initialize();
      Application.Run(new Form1());
    }
  }
}