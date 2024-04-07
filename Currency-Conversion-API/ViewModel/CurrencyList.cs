namespace Currency_Conversion_API.ViewModel
{
    public class CurrencyList
    {
        public List<Currency> currencies {  get; set; }
    }
    public class Currency
    {
        public string CurrencyCode { get; set; }
        public double CurrencyValue { get; set; }
    }

}
