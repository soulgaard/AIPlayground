using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chatbot.API
{
  public class InvoiceDto
  {
    /// <summary>
    /// Invoicing period start.
    /// </summary>
    //[BokisReport(FieldName = "Periode start")]
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Invoicing period end (last day).
    /// </summary>
    //[BokisReport(FieldName = "Periode slut")]
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Supplier Name
    /// </summary>
    [BokisReport(FieldName = "Supplier (navn)")]
    public string SUPName { get; set; }

    /// <summary>
    /// Client registration number.
    /// </summary>
    [BokisReport(FieldName = "Hreg")]
    public int ClientRegistrationNumber { get; set; }

    /// <summary>
    /// Client registration Name.
    /// </summary>
    [BokisReport(FieldName = "Hreg (navn)")]
    public string ClientRegistrationName { get; set; }
    /// <summary>
    /// REG Number
    /// </summary>
    [BokisReport(FieldName = "Reg")]
    public int? SubClientNumber { get; set; }
    /// <summary>
    /// REG Name
    /// </summary>
    [BokisReport(FieldName = "Reg (navn)")]
    public string SubClientName { get; set; }
    /// <summary>
    /// Product identifier.
    /// </summary>
    [BokisReport(FieldName = "Varenummer")]
    public string ProductIdentifier { get; set; }

    /// <summary>
    /// Product name.
    /// </summary>
    [BokisReport(FieldName = "Varetekst")]
    public string ProductName { get; set; }

    /// <summary>
    /// product idendifier and product name. Calculated.
    /// </summary>
    [BokisReport(FieldName = "Vare (nummer & tekst)")]
    public string ProductIdentifierAndName { get { return ProductIdentifier + " " + ProductName; } }

    /// <summary>
    /// Item count for line.
    /// </summary>
    [BokisReport(FieldName = "Antal")]
    public decimal ItemCount { get; set; }

    /// <summary>
    /// Item count for line.
    /// </summary>
    [BokisReport(FieldName = "Volumen")]
    public decimal? ItemVolume { get; set; }

    /// <summary>
    /// Item price.
    /// </summary>
    [BokisReport(FieldName = "Stykpris", HighPrecision = true)]
    public decimal ItemPrice { get; set; }

    /// <summary>
    /// Totale line price.
    /// </summary>
    [BokisReport(FieldName = "Totalbeløb", HighPrecision = true)]
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Transaction date.
    /// </summary>
    [BokisReport(FieldName = "Forbrugsdato")]
    public DateTime? TransactionDate { get; set; }

    /// <summary>
    /// Original line text.
    /// </summary>
    [BokisReport(FieldName = "Supplerende tekst")]
    public string LineText { get; set; }

    /// <summary>
    /// Product price group identifier
    /// </summary>
    [BokisReport(FieldName = "Service (ID)")]
    public string ProductPriceGroupIdentifier { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [BokisReport(FieldName = "Service (Navn)")]
    public string ProductPriceGroupName { get; set; }

    /// <summary>
    /// Product price group identifier and name. Calculated.
    /// </summary>
    [BokisReport(FieldName = "Service (ID & Navn)")]
    public string ProductPriceGroupIdentifierAndName { get { return ProductPriceGroupIdentifier + " " + ProductPriceGroupName; } }

    /// <summary>
    /// Brand name.
    /// </summary>
    [BokisReport(FieldName = "Brand")]
    public string BrandName { get; set; }

    /// <summary>
    /// Payment account. Format [regno]-[accountno]
    /// </summary>
    [BokisReport(FieldName = "Konto")]
    public string Account { get { return string.Format("{0:D4}-{1}", AccountRegNo, AccountAccNo); } }

    /// <summary>
    /// Product id.
    /// </summary>
    [BokisReport(Hidden = true)]
    public long PRONo { get; set; }

    /// <summary>
    /// Accountnumber for invoicing.
    /// </summary>
    [BokisReport(Hidden = true)]
    public long AccountAccNo { get; set; }

    /// <summary>
    /// Regnumber for invoicing.
    /// </summary>
    [BokisReport(Hidden = true)]
    public int AccountRegNo { get; set; }
    /// <summary>
    /// MainProduct Identifier
    /// </summary>
    [BokisReport(Hidden = true)]
    public long MAPNo { get; set; }
    /// <summary>
    /// Supplier Id
    /// </summary>
    [BokisReport(Hidden = true)]
    public long? SUPNo { get; set; }

    /// <summary>
    /// Card Product Name
    /// </summary>
    [BokisReport(FieldName = "Produktnavn")]
    public string CardProductName { get; set; }

    /// <summary>
    /// BIN
    /// </summary>
    [BokisReport(FieldName = "BIN")]
    public string BIN { get; set; }

    /// <summary>
    /// Debit / Credit
    /// </summary>
    [BokisReport(FieldName = "D/C")]
    public string DebitCredit { get; set; }

    /// <summary>
    /// Contract Description
    /// </summary>
    [BokisReport(FieldName = "Contract Description")]
    public string ContractDescription { get; set; }

    /// <summary>
    /// Original quantity from file. Used on volumebased prices, where volume is written in the quantity field
    /// </summary>
    [BokisReport(Hidden = true)]
    public decimal? ItemCountOriginal { get; set; }

    /// <summary>
    /// Charging date.
    /// </summary>
    [BokisReport(FieldName = "Opkrævningsdato")]
    public DateTime? ChargingDate { get; set; }

    /// <summary>
    /// Kørselstype
    /// </summary>
    [BokisReport(FieldName = "Kørselstype")]
    public string BillingSetType { get; set; }

    /// <summary>
    /// Afregningsvaluta
    /// </summary>
    [BokisReport(FieldName = "Afregningsvaluta")]
    public string CurrencyType { get; set; }

    /// <summary>
    /// Originalvaluta
    /// </summary>
    [BokisReport(FieldName = "Originalvaluta")]
    public string OriginalCurrencyType { get; set; }

    /// <summary>
    /// Afregningskurs
    /// </summary>
    [BokisReport(FieldName = "Afregningskurs")]
    public decimal CurrencyRate { get; set; }

    /// <summary>
    /// ORIGINAL MC CONNECT INVOICE NUMBER
    /// </summary>
    [BokisReport(FieldName = "ORIGINAL MC CONNECT INVOICE NUMBER")]
    public string MCInvoiceNumber { get; set; }
    /// <summary>
    /// MCBillingSet number, used for calculating CurrencyRate
    /// </summary>
    [BokisReport(Hidden = true)]
    public long BillingSetNo { get; set; }

    /// <summary>
    /// BC Invoice number
    /// </summary>
    [BokisReport(FieldName = "Fakturanummer")]
    public string InvoiceNumber { get; set; }

    /// <summary>
    /// BC Invoice date
    /// </summary>
    [BokisReport(FieldName = "Fakturadato")]
    public DateTime? InvoiceDate { get; set; }

    public string AsString =>
      $"Periode {PeriodStart:yyyy-MM} - " +
      $"Supplier (navn) {SUPName} - " +
      $"Hreg {ClientRegistrationNumber} ({ClientRegistrationName}) - " +
      $"Reg {SubClientNumber} ({SubClientName}) - " +
      $"Varenummer {ProductIdentifier} - " +
      $"Varetekst {ProductName} - " +
      $"Antal {ItemCount} - " +
      $"Volumen {ItemVolume?.ToString() ?? "-"} - " +
      $"Stykpris {ItemPrice} - " +
      $"Totalbeløb {TotalPrice} - " +
      $"Forbrugsdato {TransactionDate?.ToString("yyyy-MM-dd") ?? "-"} - " +
      $"Supplerende tekst {LineText} - " +
      $"Service (ID & Navn) {ProductPriceGroupIdentifierAndName} - " +
      $"Brand {BrandName} - " +
      $"Konto {Account} - " +
      $"Produktnavn {CardProductName} - " +
      $"BIN {BIN} - " +
      $"D/C {DebitCredit} - " +
      $"Contract Description {ContractDescription} - " +
      $"Opkrævningsdato {ChargingDate?.ToString("yyyy-MM-dd") ?? "-"} - " +
      $"Kørselstype {BillingSetType} - " +
      $"Afregningsvaluta {CurrencyType} - " +
      $"Originalvaluta {OriginalCurrencyType} - " +
      $"Afregningskurs {CurrencyRate} - " +
      $"ORIGINAL MC CONNECT INVOICE NUMBER {MCInvoiceNumber} - " +
      $"Fakturanummer {InvoiceNumber} - " +
      $"Fakturadato {InvoiceDate?.ToString("yyyy-MM-dd") ?? "-"}";
  }
}
