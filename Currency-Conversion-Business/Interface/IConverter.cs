using Currency_Conversion_Business.Models;

namespace Currency_Conversion_Business.Interface
{
    public interface IConverter
    {
        public Dictionary<string, double> GetCurrencies();
        public ConvertedResult DoConversion(CurrencyModel currencyModel);
        public List<HistoryModel> GetHistory(string currency);
    }
}
