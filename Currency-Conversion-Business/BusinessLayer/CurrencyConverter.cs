using Currency_Conversion_Business.Constants;
using Currency_Conversion_Business.Helper;
using Currency_Conversion_Business.Interface;
using Currency_Conversion_Business.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace Currency_Conversion_Business.BusinessLayer
{
    
    public class CurrencyConverter: IConverter
    {
        private Dictionary<string, double> _exchangeRates;
        private readonly IConfiguration _configuration;
        private readonly string _filename;
        private readonly string _dataSource;
        public CurrencyConverter(IConfiguration configuration)
        {
            //Initialize exchange rates
            _configuration = configuration;
            _dataSource = _configuration.GetSection("DataSource").Value;
            _filename = _configuration.GetSection("Filename").Value;
            UpdateCurrency();
        }

        // Method to convert amount from one currency to another
        public double Convert(double amount, string fromCurrency, string toCurrency)
        {
            if (!_exchangeRates.ContainsKey(toCurrency))
            {
                throw new ArgumentException(AppConstant.INVALIDCURRENCY_MSG);
            }
            if(Double.IsNaN(_exchangeRates[toCurrency]) || _exchangeRates[toCurrency] == 0)
                throw new ArgumentException(AppConstant.UNABLETOFINDCURRENCY_MSG);

            return amount * _exchangeRates[toCurrency];
        }

        public ConvertedResult DoConversion(CurrencyModel currencyModel)
        {
            double amount = currencyModel.CurrencyValue;
            string fromCurrency = currencyModel.FromCurrencyCode;
            string toCurrency = currencyModel.ToCurrencyCode;
            double convertedAmount = 0;
            ConvertedResult convertedResult = new ConvertedResult();
            try
            {
                 convertedAmount = Convert(amount, fromCurrency, toCurrency);
                 convertedResult.Status = 1;
                 convertedResult.Value = convertedAmount;
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == AppConstant.INVALIDCURRENCY_MSG)
                {
                    convertedResult.Status = -1;
                    convertedResult.Value = 0;
                }
                else if (ex.Message == AppConstant.UNABLETOFINDCURRENCY_MSG)
                {
                    convertedResult.Status = -2;
                    convertedResult.Value = 0;
                }

            }
            return convertedResult;

        }
        public void UpdateCurrency()
        {
            DateTime today = DateTime.Today;
            // Create the directory path using today's date
            string directoryPath = Path.Combine(Environment.CurrentDirectory + "\\"+_dataSource, today.ToString("yyyy-MM-dd"));

            // Check if the directory exists
            if (Directory.Exists(directoryPath))
            {
                string filePath = directoryPath + "\\"+_filename;
                _exchangeRates= Parser.ParseXML(filePath);
            }
        }

        public Dictionary<string, double> GetCurrencies()
        {
            return _exchangeRates;
        }

        public List<HistoryModel> GetHistory(string ToCurrency)
        {
            List<HistoryModel> historyModels = new List<HistoryModel>();
            var previousDay = GetHistory(-1, ToCurrency);
            var previousDay_2 = GetHistory(-2, ToCurrency); 
            historyModels.AddRange(previousDay);
            historyModels.AddRange(previousDay_2);
            return historyModels;
        }

       

        private List<HistoryModel> GetHistory(double days, string ToCurrency)
        {
            DateTime previousDay = DateTime.Today.AddDays(days);
            List<HistoryModel> historyModel = new List<HistoryModel>();
            string directoryPath_1 = Path.Combine(Environment.CurrentDirectory + "\\" + _dataSource, previousDay.ToString("yyyy-MM-dd"));
            Dictionary<string, double> Result;
            // Check if the directory exists
            if (Directory.Exists(directoryPath_1))
            {
                Result = new Dictionary<string, double>();
                Result = Parser.ParseXML(directoryPath_1 + "\\" + _filename);
                if (Result.ContainsKey(ToCurrency))
                {
                    historyModel.Add(new HistoryModel
                    {
                        isAvailable = true,
                        Date = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
                        message = "Fetched Successfully",
                        Result = Result[ToCurrency]
                    });
                }
            }
            return historyModel;
        }
    }
}
