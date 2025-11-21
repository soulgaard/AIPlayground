using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.API
{
  public class ProductVectorStoreRecord
  {
    [VectorStoreKey]
    public required Guid Id { get; set; }

    [VectorStoreData]
    public string ProductId { get; set; }
    [VectorStoreData]
    public string ProductName { get; set; }
    [VectorStoreData]
    public string ProductGroupId { get; set; }
    [VectorStoreData]
    public string ProductGroupName { get; set; }


    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineDistance, IndexKind = IndexKind.Flat)]
    public float[]? Embedding { get; set; }

    public string AsString =>
       $"varenummer: {ProductId} - " +
       $"varenavn {ProductName} - " +
       $"service id {ProductGroupId} - " +
       $"service navn {ProductGroupName}";
  }
}
