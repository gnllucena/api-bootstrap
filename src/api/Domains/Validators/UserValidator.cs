using System.Collections.Generic;
using API.Domains.Models;
using API.Domains.Models.Faults;
using API.Domains.Validators.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace API.Domains.Validations
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Profile)
                .NotNull()
                .WithErrorCode(((int)Validation.UserProfileNotInformed).ToString())
                .WithMessage("User's profile must be informed");

            RuleFor(x => x.Country)
                .NotNull()
                .WithErrorCode(((int)Validation.UserCountryNotInfored).ToString())
                .WithMessage("User's country must be informed");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithErrorCode(((int)Validation.UserNameNotInformed).ToString())
                .WithMessage("User's name must be informed");

            RuleFor(x => x.Name)
                .Length(1, 80)
                .WithErrorCode(((int)Validation.UserNameExceedsLimit).ToString())
                .WithMessage("User's name length must be between 1 and 80 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode(((int)Validation.UserEmailNotInformed).ToString())
                .WithMessage("User's email must be informed");

            RuleFor(x => x.Email)
                .Length(1, 80)
                .WithErrorCode(((int)Validation.UserEmailExceedsLimit).ToString())
                .WithMessage("User's name length must be between 1 and 80 characters");

            RuleFor(x => x.Email)
                .ValidEmail()
                .WithErrorCode(((int)Validation.UserEmailNotValid).ToString())
                .WithMessage("User's email is not valid");

            RuleFor(x => x.Document)
                .NotEmpty()
                .WithErrorCode(((int)Validation.UserDocumentNotInformed).ToString())
                .WithMessage("User's document must be informed");

            RuleFor(x => x.Document)
                .ValidDocument()
                .WithErrorCode(((int)Validation.UserDocumentInvalid).ToString())
                .WithMessage("User's document is not valid");

            RuleFor(x => x.Birthdate)
                .NotEmpty()
                .WithErrorCode(((int)Validation.UserBirthdateNotInformed).ToString())
                .WithMessage("User's birthdate must be informed");

            RuleFor(x => x.Birthdate)
                .ValidBirthdate()
                .WithErrorCode(((int)Validation.UserBirthdateInvalid).ToString())
                .WithMessage("User's birthdate is not valid");
        }

        protected override void EnsureInstanceNotNull(object user)
        {
            if (user == null)
            {
                var error = new ValidationFailure("User", "User must be informed", null)
                {
                    ErrorCode = ((int)Validation.UserNotInformed).ToString()
                };

                throw new ValidationException("Something happened when our server was validating your user", new List<ValidationFailure> { error });
            }
        }
    }
}
