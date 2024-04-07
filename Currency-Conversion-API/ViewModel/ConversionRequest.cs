namespace Currency_Conversion_API.ViewModel
{
    public class ConversionRequest
    {
        public string FromCurrencyCode { get; set; }
        public string ToCurrencyCode { get; set; }
        public double CurrencyValue { get; set; }

    }
}
