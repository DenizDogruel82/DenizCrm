using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Users;

public sealed class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.FullName).RequiredText(150);
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8).MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Parola büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Parola küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Parola rakam içermelidir.");
        RuleFor(x => x.Role).RequiredText(50);
    }
}

public sealed class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FullName).RequiredText(150);
        RuleFor(x => x.Role).RequiredText(50);
    }
}

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IValidator<CreateUserDto> validator) =>
        RuleFor(x => x.User).NotNull().SetValidator(validator);
}

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IValidator<UpdateUserDto> validator)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.User).NotNull().SetValidator(validator);
    }
}
