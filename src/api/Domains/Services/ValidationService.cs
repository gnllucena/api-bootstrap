using System.Collections.Generic;
using API.Domains.Models.Faults;
using FluentValidation;
using FluentValidation.Results;

namespace API.Domains.Services
{
    public interface IValidationService
    {
        void Throw(string propertyName, string errorMessage, object attemptedValue, Validation validation);
    }

    public class ValidationService : IValidationService
    {
        public void Throw(string propertyName, string errorMessage, object attemptedValue, Validation validation)
        {
            var error = new ValidationFailure(propertyName, errorMessage, attemptedValue)
            {
                ErrorCode = ((int)validation).ToString()
            };

            throw new ValidationException("Something happened when our server was validating your user", new List<ValidationFailure> { error });
        }
    }
}
