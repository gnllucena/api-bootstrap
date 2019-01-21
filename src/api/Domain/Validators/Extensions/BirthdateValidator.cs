using System;
using FluentValidation.Validators;

namespace API.Domain.Validators.Extensions
{
    public class BirthdateValidator : PropertyValidator
    {
        public BirthdateValidator() : base("{PropertyName} is not valid")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var data = (DateTime)context.PropertyValue;
            return (data >= new DateTime(1900, 1, 1) && data <= DateTime.Now);
        }
    }
}