using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.API
{
  public class InvoiceVectorStoreRecord
  {
    [VectorStoreKey]
    public required Guid Id { get; set; }
    [VectorStoreData]
    public DateTime PeriodStart { get; set; }
    [VectorStoreData]
    public string ProductIdentifier { get; set; }
    [VectorStoreData]
    public string ProductName { get; set; }
    [VectorStoreData]
    public decimal ItemCount { get; set; }
    [VectorStoreData]
    public decimal? ItemVolume { get; set; }
    [VectorStoreData]
    public decimal ItemPrice { get; set; }
    [VectorStoreData]
    public decimal TotalPrice { get; set; }
    [VectorStoreData]
    public string ProductPriceGroupName { get; set; }
    [VectorStoreData]
    public string BrandName { get; set; }
    [VectorStoreData]
    public string BillingSetType { get; set; }

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineDistance, IndexKind = IndexKind.Flat)]
    public float[]? Embedding { get; set; }

    public string GetAsString()
    {
      return $"Date: {PeriodStart} - " +
        $"Amount: {TotalPrice} - " +
        $"Item id: {ProductIdentifier} - " +
        $"item name: {ProductName} - " +
        $"Brand: {BrandName} - " +
        $"Group: {ProductPriceGroupName} - " +
        $"Count: {ItemCount} - " +
        $"total: {TotalPrice:n0}";
    }
    //public string? Embedding => $"Dato: {PeriodStart} - Amount: {TotalPrice} - Varenummer: {ProductIdentifier} - varenavn: {ProductName}";
  }
}
