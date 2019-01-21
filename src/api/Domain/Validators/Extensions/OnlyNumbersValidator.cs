using FluentValidation.Validators;

namespace API.Domain.Validators.Extensions
{
    public class OnlyNumbersValidator : PropertyValidator
    {
        public OnlyNumbersValidator() : base("{PropertyName} is not valid")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var texto = context.PropertyValue as string;

            if (!string.IsNullOrWhiteSpace(texto))
            {
                return long.TryParse(texto, out long valor);
            }

            return false;
        }
    }
}
