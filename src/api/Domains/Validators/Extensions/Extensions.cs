using FluentValidation;

namespace API.Domains.Validators.Extensions
{
    public static class Extensions
    {
        public static IRuleBuilderOptions<T, TElement> ValidDocument<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DocumentValidator());
        }

        public static IRuleBuilderOptions<T, TElement> ValidEmail<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new EmailValidator());
        }

        public static IRuleBuilderOptions<T, TElement> ValidBirthdate<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new BirthdateValidator());
        }

        public static IRuleBuilderOptions<T, TElement> OnlyNumbers<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new OnlyNumbersValidator());
        }
    }
}
