using Currency_Conversion_API.ViewModel;
using FluentValidation;

namespace Currency_Conversion_API.Validation
{
    public class RequestValidation : AbstractValidator<ConversionRequest>
    {
        public RequestValidation()
        {
            RuleFor(x => x.ToCurrencyCode).NotEmpty().WithMessage("To Currency Code is required.");
            RuleFor(x => x.FromCurrencyCode).NotEmpty().WithMessage("From Currency Code is required.");
            RuleFor(x => x.CurrencyValue).GreaterThan(0).WithMessage("Currency Value Must be Greater than 0");
        }
    }
}
