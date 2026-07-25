using FluentValidation;

namespace SaasAiCrm.Application.Common.Validation;

internal static class ValidationRules
{
    public static IRuleBuilderOptions<T, string> RequiredText<T>(
        this IRuleBuilder<T, string> rule,
        int maximumLength) =>
        rule.NotEmpty()
            .WithMessage("'{PropertyName}' alanı zorunludur.")
            .MaximumLength(maximumLength)
            .WithMessage("'{PropertyName}' en fazla {MaxLength} karakter olabilir.");

    public static IRuleBuilderOptions<T, string?> OptionalText<T>(
        this IRuleBuilder<T, string?> rule,
        int maximumLength) =>
        rule.MaximumLength(maximumLength)
            .WithMessage("'{PropertyName}' en fazla {MaxLength} karakter olabilir.");

    public static IRuleBuilderOptions<T, string?> OptionalEmail<T>(
        this IRuleBuilder<T, string?> rule) =>
        rule.MaximumLength(254)
            .EmailAddress()
            .WithMessage("Geçerli bir e-posta adresi girilmelidir.");
}
