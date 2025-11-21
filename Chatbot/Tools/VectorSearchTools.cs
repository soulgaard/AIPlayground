using Chatbot.API;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace Chatbot.Tools
{
  public class VectorSearchTools
  {
    //private readonly InMemoryCollection<Guid, InvoiceVectorStoreRecord> _invoiceCollection;
    private readonly InMemoryCollection<Guid, ProductVectorStoreRecord> _productCollection;

    public VectorSearchTools(
      //InMemoryCollection<Guid, InvoiceVectorStoreRecord> invoiceCollection,
      InMemoryCollection<Guid, ProductVectorStoreRecord> productCollection
      )
    {
      //_invoiceCollection = invoiceCollection;
      _productCollection = productCollection;
    }

    // Vi vil helst ikke have AI laver vectorsøgning i invoices, da vectorsøgning er bedst til tekstbaseret data
    /*
    private async Task<List<string>> SearchInvoiceVectorStore(string question, int? count = 10)
    {
      List<string> result = [];
      await foreach (VectorSearchResult<InvoiceVectorStoreRecord> searchResult in _invoiceCollection.SearchAsync(question, count ?? 10,
                         new VectorSearchOptions<InvoiceVectorStoreRecord>
                         {
                           IncludeVectors = false
                         }))
      {
        InvoiceVectorStoreRecord record = searchResult.Record;
        result.Add(record.GetAsString());
      }

      return result;
    }
    */

    public async Task<List<string>> SearchProductListVectorStore(string question, int? count = 10)
    {
      List<string> result = [];
      await foreach (VectorSearchResult<ProductVectorStoreRecord> searchResult in _productCollection.SearchAsync(question, count ?? 10,
                         new VectorSearchOptions<ProductVectorStoreRecord>
                         {
                           IncludeVectors = false
                         }))
      {
        ProductVectorStoreRecord record = searchResult.Record;
        result.Add(record.AsString);
      }

      return result;
    }

  }
}
