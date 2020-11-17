using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace Common.Services
{
    public interface IValidationService
    {
        void Validate(ValidationResult result, List<(bool Exists, string Property, object Value, string Message)> validations);
    }

    public class ValidationService : IValidationService
    {
        public void Validate(ValidationResult result, List<(bool Exists, string Property, object Value, string Message)> validations)
        {
            foreach (var validation in validations)
            {
                if (validation.Exists)
                {
                    result.Errors.Add(new ValidationFailure(validation.Property, validation.Message, validation.Value));
                }
            }

            if (result.Errors.Count > 0)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}