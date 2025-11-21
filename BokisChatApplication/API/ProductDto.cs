namespace Chatbot.API
{
  public class ProductDto
  {
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductGroupId { get; set; }
    public string ProductGroupName { get; set; }

    public string AsString =>
      $"varenummer: {ProductId} - " +
      $"varenavn {ProductName} - " +
      $"service id {ProductGroupId} - " +
      $"service navn {ProductGroupName}";
  }
}