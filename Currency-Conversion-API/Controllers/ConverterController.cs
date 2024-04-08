using AutoMapper;
using Currency_Conversion_Business.Interface;
using Currency_Conversion_Business.Models;
using Currency_Conversion_API.ViewModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Currency_Conversion_API.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        IConverter currencyConverter;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConverterController> _logger;
        public ConverterController(IConverter convertor, IMapper mapper, IConfiguration configuration, ILogger<ConverterController> logger)
        {
            _configuration = configuration;
            currencyConverter = convertor;
            _mapper = mapper;
            _logger = logger;
        }
        // GET: api/<ConverterController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var currencies=currencyConverter.GetCurrencies();
            List<Currency> currencyList = new List<Currency>();
            foreach (var cur in currencies)
            {
                currencyList.Add(new Currency()
                {
                    CurrencyCode = cur.Key,
                    CurrencyValue = cur.Value
                });
            }
            _logger.LogInformation("Get method invocked successfully");
            return Ok(currencyList);
        }

        // POST api/<ConverterController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ConversionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CurrencyModel currencyModel = new CurrencyModel()
            {
                ToCurrencyCode=request.ToCurrencyCode,
                FromCurrencyCode=request.FromCurrencyCode,
                CurrencyValue=request.CurrencyValue,
            };

            var convertedResult= currencyConverter.DoConversion(currencyModel);
            var value=convertedResult.Value;
            if (convertedResult.Status == -1)
            {
                _logger.LogInformation("Post method invocked: Invalid Currency");

                return BadRequest(new ConversionResult()
                {
                    isSuccess = false,
                    Result = -1,
                    message = "Invalid Currency",
                    histories = null
                });
            }
            if (convertedResult.Status == -2)
            {
                _logger.LogInformation("Post method invocked: Unable to Fetch Currency");

                return Ok(new ConversionResult()
                {
                    isSuccess = false,
                    Result = -1,
                    message = "Unable to Fetch Currency",
                    histories = null
                });
            }

            var historyList=currencyConverter.GetHistory(request.ToCurrencyCode);
            var histories = _mapper.Map<List<History>>(historyList);
            var result= new ConversionResult()
            {
                isSuccess=true,
                Result = value,
                message ="Success",
                histories= histories
            };
            _logger.LogInformation("Post method invocked successfully");

            return Ok(result);

        }
        
    }
}
