using Azure.AI.OpenAI;
using Chatbot.API;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chatbot.Tools
{
  public class ToolHelper
  {
    Microsoft.Extensions.AI.IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    Microsoft.SemanticKernel.Connectors.InMemory.InMemoryVectorStore _vectorStore;
    private BokisAPI _api;
    private AzureOpenAIClient _client;
    private string? _embeddingModel;

    public ToolHelper(BokisAPI api, AzureOpenAIClient client, string embeddingModel)
    {
      _api = api;
      _client = client;
      _embeddingModel = embeddingModel;

      _embeddingGenerator = client
     .GetEmbeddingClient(_embeddingModel)
     .AsIEmbeddingGenerator();
      
      _vectorStore =
         new(new InMemoryVectorStoreOptions
         {
           EmbeddingGenerator = _embeddingGenerator
         });
    }

    public async Task<List<AITool>> CreateToolListAsync()
    {
      #region Embed and store Product vector store
      var filename = "C:\\_WS\\AISandbox\\MovieExample\\Data\\ProductVectorStore.json";
      var forceRegeneration = false;

      _api.EnsureDataLoaded();

      InMemoryCollection<Guid, ProductVectorStoreRecord> productCollection =
        _vectorStore.GetCollection<Guid, ProductVectorStoreRecord>("products");
      await productCollection.EnsureCollectionExistsAsync();

      if (File.Exists(filename) && !forceRegeneration)
      {
        var json = await File.ReadAllTextAsync(filename);
        var records = JsonSerializer.Deserialize<List<ProductVectorStoreRecord>>(json);

        foreach (var record in records)
        {
          await productCollection.UpsertAsync(record); // embeddings already included
        }

        Utils.WriteLineDarkGray("Loaded embeddings from file.");
      }
      else
      {
        Utils.WriteLineDarkGray("No embedded product data found. Embedding...");

        int counter = 0;
        var persistedRecords = new List<ProductVectorStoreRecord>();
        // Embed
        foreach (var record in _api.ProductList)
        {
          counter++;
          Console.Write(".");
          if (counter % 50 == 0) Console.WriteLine();
          if (counter % 1000 == 0) Console.WriteLine($"--- {counter} ---");

          var text = record.AsString;
          var embedding = await _embeddingGenerator.GenerateAsync(text);
          var vectorRecord = new ProductVectorStoreRecord
          {
            Id = Guid.NewGuid(),
            ProductGroupId = record.ProductGroupId,
            ProductGroupName = record.ProductGroupName,
            ProductId = record.ProductId,
            ProductName = record.ProductName,
            Embedding = embedding.Vector.ToArray()
          };

          await productCollection.UpsertAsync(vectorRecord);
          persistedRecords.Add(vectorRecord);
        }

        var json = JsonSerializer.Serialize(persistedRecords, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filename, json);
      }
      #endregion

      var tools = new QueryTools(_api.InvoiceList, _api.ProductList);
      var methods = typeof(QueryTools).GetMethods(BindingFlags.Public | BindingFlags.Instance);
      var toolList = methods.Select(l => AIFunctionFactory.Create(l, tools)).Cast<AITool>().ToList();

      var vectorTools = new VectorSearchTools(productCollection);
      methods = typeof(VectorSearchTools).GetMethods(BindingFlags.Public | BindingFlags.Instance);
      toolList.AddRange(methods.Select(l => AIFunctionFactory.Create(l, vectorTools)).Cast<AITool>().ToList());

      var graphTools = new GraphTools();
      methods = typeof(GraphTools).GetMethods(BindingFlags.Public | BindingFlags.Instance);
      toolList.AddRange(methods.Select(l => AIFunctionFactory.Create(l, graphTools)).Cast<AITool>().ToList());

      Utils.WriteLineDarkGray($"Toollist created ({toolList.Count} tools)");

      return toolList;
    }
  }
}
