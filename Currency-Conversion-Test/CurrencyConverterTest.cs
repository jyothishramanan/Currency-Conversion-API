using Currency_Conversion_Business.BusinessLayer;
using Currency_Conversion_Business.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;

namespace Currency_Conversion_Test
{
    public class CurrencyConverterTest
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly CurrencyConverter _converter;
        private Dictionary<string, double> _mockData;

        public CurrencyConverterTest()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            //SetUpMockData();
            // Setup mock configuration
            _mockConfiguration.Setup(c => c.GetSection("DataSource").Value).Returns("C:\\Work\\Currency-App\\Currency-Conversion-API\\Currency-Conversion-API\\CurrencySource");
            _mockConfiguration.Setup(c => c.GetSection("Filename").Value).Returns("data.xml");

            // Assume we have a method to setup mock data


            _converter = new CurrencyConverter(_mockConfiguration.Object);
        }
        public void SetUpMockData()
        {
            string serviceResponse = "[{\"isAvailable\":true,\"Result\":1.0841,\"message\":\"Fetched Successfully\",\"Date\":\"2024-04-05\"},{\"isAvailable\":true,\"Result\":1.0852,\"message\":\"Fetched Successfully\",\"Date\":\"2024-04-04\"}]";
            ConvertedResult convertedResult = new ConvertedResult() { Value = 1.0841, Status = 1 };
            _mockData = new Dictionary<string, double>();
            _mockData = JsonConvert.DeserializeObject<Dictionary<string, double>>(serviceResponse);
        }

        [Fact]
        public void Convert_WithValidCurrencies_ReturnsConvertedValue()
        {
            // Arrange
            double expected = 200; // Assuming conversion rate is 2.0 for simplicity

            // Act
            double result = _converter.Convert(100, "EUR", "USD");

            // Assert
            Assert.Equal(expected, result);
        }
    }
}