using Chatbot.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.Tools
{
  public class QueryTools
  {
    private List<InvoiceDto> _invoiceList;
    private List<ProductDto> _productList;

    public QueryTools(List<InvoiceDto> invoiceList, List<ProductDto> productList)
    {
      _invoiceList = invoiceList;
      _productList = productList;
    }

    /// <summary>
    /// Query product list with optional filters.
    /// </summary>
    /// <param name="filterByProduct">Varetekst</param>
    /// <param name="filterByService">Service navn</param>
    /// <returns>String with filtered product list</returns>
    public async Task<string> QueryProductList(
      string? filterByProduct = null,
      string? filterByService = null
      )
    {
      var query = _productList.AsQueryable();

      // Apply filters
      if (!string.IsNullOrEmpty(filterByProduct))
        query = query.Where(p => p.ProductName.Contains(filterByProduct, StringComparison.OrdinalIgnoreCase));

      if (!string.IsNullOrEmpty(filterByService))
        query = query.Where(p => p.ProductGroupName.Contains(filterByService, StringComparison.OrdinalIgnoreCase));

      // Return formatted string list
      var res = string.Join(Environment.NewLine, query.Select(l => l.AsString).ToList());
      return res;
    }

    #region Aggregation enums and record types
    public enum AggregateFunction { Sum, Avg, Count }

    /// <summary>
    /// Defines which numeric field to aggregate.
    /// </summary>
    public enum AggregateField
    {
      TotalPrice,
      ItemVolume,
      ItemCount
    }

    /// <summary>
    /// Defines available grouping fields.
    /// </summary>
    public enum GroupByField
    {
      None,
      Brand,      
      Product,
      ProductId,
      ServiceId,
      Service,
      Period,
      TransactionDate,
      CardProductName,
      DebitCredit,
      Supplier
    }

    /// <summary>
    /// store result of aggregation by aggregation key and the aggregated value.
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Value"></param>
    public record AggregateResult(string Key, decimal Value);
    #endregion

    /// <summary>
    /// Aggregates invoice data with flexible filtering, grouping, and aggregation options.
    /// </summary>
    /// <param name="filterByBrands">Optional list of brand names to filter by (case-insensitive).</param>
    /// <param name="filterByProducts">Optional list of product names to filter by (case-insensitive).</param>
    /// <param name="filterByProductIds">Optional list of product identifiers to filter by (case-insensitive).</param>
    /// <param name="filterByServiceIds">Optional list of service identifiers to filter by (case-insensitive).</param>
    /// <param name="filterByServices">Optional list of service names to filter by (case-insensitive).</param>
    /// <param name="periodStart">Optional start date for filtering invoices by period.</param>
    /// <param name="periodEnd">Optional end date for filtering invoices by period.</param>
    /// <param name="minItems">Optional minimum item count filter.</param>
    /// <param name="aggregate">Specifies the aggregation function (Sum, Avg, Count).</param>
    /// <param name="groupBy">Specifies the grouping field (Brand, Product, ProductId, ServiceId, Service, Period, TransactionDate, CardProductName, DebitCredit).</param>
    /// <param name="aggregateField">Specifies which numeric field to aggregate (TotalPrice, ItemVolume, ItemCount).</param>
    /// <returns>A collection of <see cref="AggregateResult"/> containing grouped keys and aggregated values.</returns>
    public async Task<IEnumerable<AggregateResult>> AggregateInvoiceList(
        IEnumerable<string>? filterByBrands = null,
        IEnumerable<string>? filterByProducts = null,
        IEnumerable<string>? filterByProductIds = null,
        IEnumerable<string>? filterByServiceIds = null,
        IEnumerable<string>? filterByServices = null,
        DateTime? periodStart = null,
        DateTime? periodEnd = null,
        int? minItems = null,
        AggregateFunction aggregate = AggregateFunction.Sum,
        GroupByField groupBy = GroupByField.None,
        AggregateField aggregateField = AggregateField.TotalPrice)
    {
      var query = _invoiceList.AsQueryable();

      // Normalize filters (case-insensitive)
      if (filterByBrands?.Any() == true)
      {
        var brands = filterByBrands.ToHashSet(StringComparer.OrdinalIgnoreCase);
        query = query.Where(i => brands.Contains(i.BrandName));
      }

      if (filterByProducts?.Any() == true)
      {
        var products = filterByProducts.ToHashSet(StringComparer.OrdinalIgnoreCase);
        query = query.Where(i => products.Contains(i.ProductName));
      }

      if (filterByProductIds?.Any() == true)
      {
        var productIds = filterByProductIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        query = query.Where(i => productIds.Contains(i.ProductIdentifier));
      }

      if (filterByServiceIds?.Any() == true)
      {
        var serviceIds = filterByServiceIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        query = query.Where(i => serviceIds.Contains(i.ProductPriceGroupIdentifier));
      }

      if (filterByServices?.Any() == true)
      {
        var services = filterByServices.ToHashSet(StringComparer.OrdinalIgnoreCase);
        query = query.Where(i => services.Contains(i.ProductPriceGroupName));
      }

      // Period interval filter
      if (periodStart.HasValue)
        query = query.Where(i => i.PeriodStart >= periodStart.Value);
      if (periodEnd.HasValue)
        query = query.Where(i => i.PeriodEnd <= periodEnd.Value);

      if (minItems.HasValue)
        query = query.Where(i => i.ItemCount >= minItems.Value);

      // Grouping
      Func<InvoiceDto, string> groupKeySelector = i => "All";

      switch (groupBy)
      {
        case GroupByField.Brand:
          groupKeySelector = i => i.BrandName;
          break;
        case GroupByField.Product:
          groupKeySelector = i => i.ProductName;
          break;
        case GroupByField.ProductId:
          groupKeySelector = i => i.ProductIdentifier;
          break;
        case GroupByField.ServiceId:
          groupKeySelector = i => i.ProductPriceGroupIdentifier;
          break;
        case GroupByField.Service:
          groupKeySelector = i => i.ProductPriceGroupName;
          break;
        case GroupByField.Period:
          groupKeySelector = i => i.PeriodStart.ToString("yyyy-MM");
          break;
        case GroupByField.TransactionDate:
          groupKeySelector = i => i.TransactionDate?.ToString("yyyy-MM-dd") ?? "No date";
          break;
        case GroupByField.CardProductName:
          groupKeySelector = i => i.CardProductName;
          break;
        case GroupByField.DebitCredit:
          groupKeySelector = i => i.DebitCredit; // "debit" or "credit"
          break;
        case GroupByField.Supplier:
          groupKeySelector = i => i.SUPName;
          break;
      }

      // Aggregation field selector
      Func<InvoiceDto, decimal> valueSelector = aggregateField switch
      {
        AggregateField.TotalPrice => i => i.TotalPrice,
        AggregateField.ItemVolume => i => i.ItemVolume ?? 0,
        AggregateField.ItemCount => i => i.ItemCount,
        _ => i => i.TotalPrice
      };

      if (groupBy != GroupByField.None)
      {
        var grouped = query.GroupBy(groupKeySelector);

        return grouped.Select(g =>
        {
          decimal aggValue = aggregate switch
          {
            AggregateFunction.Sum => g.Sum(valueSelector),
            AggregateFunction.Avg => g.Average(valueSelector),
            AggregateFunction.Count => g.Count(),
            _ => g.Sum(valueSelector)
          };
          return new AggregateResult(g.Key, aggValue);
        }).ToList();
      }
      else
      {
        // No grouping, just aggregate whole set
        decimal aggValue = aggregate switch
        {
          AggregateFunction.Sum => query.Sum(valueSelector),
          AggregateFunction.Avg => query.Average(valueSelector),
          AggregateFunction.Count => query.Count(),
          _ => query.Sum(valueSelector)
        };

        return new List<AggregateResult> { new AggregateResult("All", aggValue) };
      }
    }


  }
}
