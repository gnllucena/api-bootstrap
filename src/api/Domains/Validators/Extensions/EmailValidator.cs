using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace API.Domains.Validators.Extensions
{
    public class EmailValidator : PropertyValidator
    {
        public EmailValidator() : base("{PropertyName} is not valid")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var email = context.PropertyValue as string;

            if (!string.IsNullOrWhiteSpace(email))
            {
                var match = Regex.Match(email, "[\\w-]+@([\\w-]+\\.)+[\\w-]+");

                return match.Success;
            }

            return false;
        }
    }
}
