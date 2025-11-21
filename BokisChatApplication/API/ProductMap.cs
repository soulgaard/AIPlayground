namespace Chatbot.API
{
  public class ProductMap : CsvHelper.Configuration.ClassMap<ProductDto>
  {
    public ProductMap()
    {
      Map(m => m.ProductId).Name("varenummer");
      Map(m => m.ProductName).Name("varetekst");
      Map(m => m.ProductGroupId).Name("Service (ID)");
      Map(m => m.ProductGroupName).Name("Service (Navn)");
    }
  }
}