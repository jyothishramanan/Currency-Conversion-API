using AutoMapper;
using Currency_Conversion_API.Controllers;
using Currency_Conversion_Business.Helper;
using Currency_Conversion_Business.Interface;
using Currency_Conversion_Business.Models;
using Currency_Conversion_API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Currency_Conversion_Test
{
    public class CurrencyControllerTest
    {
        IMapper _mapper;
        private Mock<IConverter> _convertor;
        private Mock<IConfiguration> _mockConfig;
        private Mock<ILogger<ConverterController>> _mockLog;
        ILogger<ConverterController> logger;
        public CurrencyControllerTest()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new CustomMapper());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
            _convertor = new Mock<IConverter>();
            _mockConfig = new Mock<IConfiguration>();
            _mockLog = new Mock<ILogger<ConverterController>>();
             logger = _mockLog.Object;

        }
        [Fact]
        public void Get_Test()
        {
            string serviceResponse = "{\r\n  \"USD\": 1.0841,\r\n  \"JPY\": 164.1,\r\n  \"BGN\": 1.9558,\r\n  \"CZK\": 25.286,\r\n  \"DKK\": 7.459,\r\n  \"GBP\": 0.85773,\r\n  \"HUF\": 390.1,\r\n  \"PLN\": 4.2835,\r\n  \"RON\": 4.9677,\r\n  \"SEK\": 11.526,\r\n  \"CHF\": 0.9793,\r\n  \"ISK\": 150.3,\r\n  \"NOK\": 11.6118,\r\n  \"TRY\": 34.6312,\r\n  \"AUD\": 1.6461,\r\n  \"BRL\": 5.4633,\r\n  \"CAD\": 1.4702,\r\n  \"CNY\": 7.8421,\r\n  \"HKD\": 8.4868,\r\n  \"IDR\": 17192.63,\r\n  \"ILS\": 4.0725,\r\n  \"INR\": 90.3283,\r\n  \"KRW\": 1464.09,\r\n  \"MXN\": 17.9104,\r\n  \"MYR\": 5.1462,\r\n  \"NZD\": 1.8001,\r\n  \"PHP\": 61.268,\r\n  \"SGD\": 1.4608,\r\n  \"THB\": 39.743,\r\n  \"ZAR\": 20.171\r\n}";
            var values = JsonConvert.DeserializeObject<Dictionary<string, double>>(serviceResponse);


            _convertor.Setup(x => x.GetCurrencies()).Returns(values);
            var controller = new ConverterController(_convertor.Object, _mapper, _mockConfig.Object, logger);
            var result = (ObjectResult)controller.Get().Result;
            List<Currency> currencyList = (List<Currency>)result.Value;
            Assert.NotNull(currencyList);
            Assert.Equal(1.0841, currencyList[0].CurrencyValue);
            Assert.Equal(200, result.StatusCode);

        }
        [Fact]
        public void Post_Test()
        {
            string serviceResponse = "[{\"isAvailable\":true,\"Result\":1.0841,\"message\":\"Fetched Successfully\",\"Date\":\"2024-04-05\"},{\"isAvailable\":true,\"Result\":1.0852,\"message\":\"Fetched Successfully\",\"Date\":\"2024-04-04\"}]";
            ConvertedResult convertedResult = new ConvertedResult() { Value = 1.0841, Status = 1 };
            var history = JsonConvert.DeserializeObject<List<HistoryModel>>(serviceResponse);
            _convertor.Setup(x => x.DoConversion(It.IsAny<CurrencyModel>())).Returns(convertedResult);
            _convertor.Setup(x => x.GetHistory(It.IsAny<string>())).Returns(history); ;

            var controller = new ConverterController(_convertor.Object, _mapper, _mockConfig.Object, logger);
            var result = (ObjectResult)controller.Post(new ConversionRequest()
            {
                FromCurrencyCode = "EURO",
                ToCurrencyCode = "USD",
                CurrencyValue = 1

            }).Result;
            ConversionResult currencyList = (ConversionResult)result.Value;
            Assert.NotNull(currencyList);

        }
        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new ConverterController(_convertor.Object, _mapper, _mockConfig.Object, logger);

            var request = new ConversionRequest(); // Assuming this will not meet your validation requirements
            controller.ModelState.AddModelError("AnError", "Some error description"); // Simulating an invalid model state

            // Act
            var result = await controller.Post(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequestResult = (BadRequestObjectResult)result;
            badRequestResult.StatusCode.Should().Be(400); // Optional: Check if you want to assert on the status code as well

            // You can further assert that the content of the BadRequest (e.g., validation messages) is as expected.
            // This part is optional and depends on how detailed you want your test to be.
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_ForInvalidCurrency()
        {
            // Arrange
            var controller = new ConverterController(_convertor.Object, _mapper, _mockConfig.Object, logger);

            var request = new ConversionRequest
            {
                FromCurrencyCode = "XXX", // Assuming "XXX" is an invalid currency code
                ToCurrencyCode = "YYY",
                CurrencyValue = 100
            };

            // Simulate DoConversion indicating an invalid currency
            _convertor.Setup(x => x.DoConversion(It.IsAny<CurrencyModel>()))
                         .Returns(new ConvertedResult { Status = -1, Value = 0 });

            // Act
            var result = await controller.Post(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = (BadRequestObjectResult)result;
            var conversionResult = badRequestResult.Value as ConversionResult;
            conversionResult.Should().NotBeNull();
            conversionResult.isSuccess.Should().BeFalse();
            conversionResult.message.Should().Contain("Invalid Currency");
        }

        [Fact]
        public async Task Post_ReturnsOk_WithFailureMessage_WhenCurrencyRateFetchFails()
        {
            // Arrange
            var controller = new ConverterController(_convertor.Object, _mapper, _mockConfig.Object, logger);

            var request = new ConversionRequest
            {
                FromCurrencyCode = "USD",
                ToCurrencyCode = "EUR",
                CurrencyValue = 100
            };

            // Simulate DoConversion failing to fetch currency rates
            _convertor.Setup(x => x.DoConversion(It.IsAny<CurrencyModel>()))
                         .Returns(new ConvertedResult { Status = -2, Value = 0 });

            // Act
            var result = await controller.Post(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            var conversionResult = okResult.Value as ConversionResult;
            conversionResult.Should().NotBeNull();
            conversionResult.isSuccess.Should().BeFalse();
            conversionResult.message.Should().Contain("Unable to Fetch Currency");
        }


    }
}
