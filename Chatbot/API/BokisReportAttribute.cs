
namespace Chatbot.API
{
  internal class BokisReportAttribute : Attribute
  {
    public string FieldName { get; set; }
    public bool Hidden { get; set; }
    public bool HighPrecision { get; set; }
  }
}