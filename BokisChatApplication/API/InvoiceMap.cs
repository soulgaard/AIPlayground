using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.API
{
    public sealed class InvoiceMap : CsvHelper.Configuration.ClassMap<InvoiceDto>
  {
    public InvoiceMap()
    {
      Map(m => m.PeriodStart).Name("Periode start");
      Map(m => m.PeriodEnd).Name("Periode slut");
      Map(m => m.SUPName).Name("Supplier (navn)");
      Map(m => m.ClientRegistrationNumber).Name("Hreg");
      Map(m => m.ClientRegistrationName).Name("Hreg (navn)");
      Map(m => m.SubClientNumber).Name("Reg");
      Map(m => m.SubClientName).Name("Reg (navn)");
      Map(m => m.ProductIdentifier).Name("Varenummer");
      Map(m => m.ProductName).Name("Varetekst");
      Map(m => m.ProductIdentifierAndName).Name("Vare (nummer & tekst)");
      Map(m => m.ItemCount).Name("Antal");
      Map(m => m.ItemVolume).Name("Volumen");
      Map(m => m.ItemPrice).Name("Stykpris");
      Map(m => m.TotalPrice).Name("Totalbeløb");
      Map(m => m.TransactionDate).Name("Forbrugsdato");
      Map(m => m.LineText).Name("Supplerende tekst");
      Map(m => m.ProductPriceGroupIdentifier).Name("Service (ID)");
      Map(m => m.ProductPriceGroupName).Name("Service (Navn)");
      Map(m => m.ProductPriceGroupIdentifierAndName).Name("Service (ID & Navn)");
      Map(m => m.BrandName).Name("Brand");
      Map(m => m.Account).Name("Konto");
      Map(m => m.CardProductName).Name("Produktnavn");
      Map(m => m.BIN).Name("BIN");
      Map(m => m.DebitCredit).Name("D/C");
      Map(m => m.ContractDescription).Name("Contract Description");
      Map(m => m.ChargingDate).Name("Opkrævningsdato");
      Map(m => m.BillingSetType).Name("Kørselstype");
      Map(m => m.CurrencyType).Name("Afregningsvaluta");
      Map(m => m.OriginalCurrencyType).Name("Originalvaluta");
      Map(m => m.CurrencyRate).Name("Afregningskurs");
      Map(m => m.MCInvoiceNumber).Name("ORIGINAL MC CONNECT INVOICE NUMBER");
      Map(m => m.InvoiceNumber).Name("Fakturanummer");
      Map(m => m.InvoiceDate).Name("Fakturadato");
    }
  }
}
