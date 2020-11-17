using Common.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace Common.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name)
                .MinimumLength(1)
                .WithMessage(x => $"The length of User's Name must be at least 1 characters");

            RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage(x => $"The length of User's Name must be 100 characters or fewer");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(x => $"The User's Name is required");

            RuleFor(x => x.Email)
                .MinimumLength(5)
                .WithMessage(x => $"The length of User's Email must be at least 5 characters");

            RuleFor(x => x.Email)
                .MaximumLength(100)
                .WithMessage(x => $"The length of User's Email must be 100 characters or fewer");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(x => $"The User's Email is required");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(x => $"The User's Email is not a valid email address");

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .WithMessage(x => $"The length of User's Password must be at least 8 characters");

            RuleFor(x => x.Password)
                .MaximumLength(30)
                .WithMessage(x => $"The length of User's Password must be 30 characters or fewer");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(x => $"The User's Password is required");

            RuleFor(x => x.Created)
                .NotEmpty()
                .WithMessage(x => $"The User's Created is required");

            RuleFor(x => x.Created)
                .Must(x => x < DateTime.Now)
                .WithMessage(x => $"The User's Created must be a date in the past");

            RuleFor(x => x.CreatedBy)
                .NotEmpty()
                .WithMessage(x => $"The User's CreatedBy is required");

            RuleFor(x => x.Updated)
                .NotEmpty()
                .When(x => x.Id > 0)
                .WithMessage(x => $"The User's Updated is required");

            RuleFor(x => x.Updated)
                .Must(x => x < DateTime.Now)
                .When(x => x.Id > 0)
                .WithMessage(x => $"The User's Updated must be a date in the past");

            RuleFor(x => x.Updated)
                .Empty()
                .When(x => x.Id == 0)
                .WithMessage(x => $"The User's Updated must not be filled");

            RuleFor(x => x.UpdatedBy)
                .NotEmpty()
                .When(x => x.Id > 0)
                .WithMessage(x => $"The User's UpdatedBy is required");

            RuleFor(x => x.UpdatedBy)
                .Empty()
                .When(x => x.Id == 0)
                .WithMessage(x => $"The User's UpdatedBy must not be filled");
        }

        protected override void EnsureInstanceNotNull(object user)
        {
            if (user == null)
            {
                var failure = new ValidationFailure("User", "User must be informed", null);

                throw new ValidationException("Something happend when validating User", new List<ValidationFailure> { failure });
            }
        }
    }
}
