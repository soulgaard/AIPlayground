using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text;

namespace Chatbot.API
{
  /// <summary>
  /// Fake API to provide data from Bokis Datastore
  /// </summary>
  public class BokisAPI
  {
    private List<InvoiceDto> _invoiceDtoList;
    private List<ProductDto> _productDtoList;

    public List<InvoiceDto> InvoiceList
    {
      get
      {
        EnsureDataLoaded();
        return _invoiceDtoList;
      }
    }

    public List<ProductDto> ProductList
    {
      get
      {
        EnsureDataLoaded();
        return _productDtoList;
      }
    }

    public void EnsureDataLoaded()
    {
      if (
        _invoiceDtoList == null ||
        !_invoiceDtoList.Any() ||
        _productDtoList == null ||
        !_productDtoList.Any()
        )
      {
        LoadData();
      }
    }

    private CsvConfiguration GetCsvConfig()
    {
      var config = new CsvConfiguration(CultureInfo.GetCultureInfo("da-DK"))
      {
        Delimiter = ";",                                 // very important!
        HasHeaderRecord = true,
        DetectDelimiter = false,
        TrimOptions = TrimOptions.Trim,                  // remove spaces
        IgnoreBlankLines = true,
        BadDataFound = null,
        MissingFieldFound = null,
        HeaderValidated = null,
        PrepareHeaderForMatch = args => args.Header.Trim().TrimStart('\uFEFF'),
      };

      return config;
    }

    private void LoadData()
    {
      var config = GetCsvConfig();

      ReadInvoiceData(config);
      ReadProductData(config);
    }

    private void ReadInvoiceData(CsvConfiguration config)
    {
      using var reader = new StreamReader(@"Data\Data2024.csv", Encoding.Unicode);
      using var csv = new CsvReader(reader, config);
      csv.Context.RegisterClassMap<InvoiceMap>();
      _invoiceDtoList = csv.GetRecords<InvoiceDto>().ToList();
    }
    private void ReadProductData(CsvConfiguration config)
    {
      using var reader = new StreamReader(@"Data\ProductList.csv", Encoding.Unicode);
      using var csv = new CsvReader(reader, config);
      csv.Context.RegisterClassMap<ProductMap>();
      _productDtoList = csv.GetRecords<ProductDto>().ToList();
    }
  }
}
