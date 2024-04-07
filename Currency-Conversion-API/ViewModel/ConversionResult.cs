namespace Currency_Conversion_API.ViewModel
{
    public class ConversionResult
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public double? Result { get; set; }
        public List<History> histories { get; set; }
    }
    public class History
    {
        public bool isAvailable { get; set; }
        public double? Result { get; set; }
        public string message { get; set; }
        public string Date { get; set; }

    }
}
