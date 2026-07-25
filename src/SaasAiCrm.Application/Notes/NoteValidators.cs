using FluentValidation;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Validation;

namespace SaasAiCrm.Application.Notes;

public sealed class CreateNoteDtoValidator : AbstractValidator<CreateNoteDto>
{
    public CreateNoteDtoValidator()
    {
        RuleFor(x => x.Content).RequiredText(5000);
        RuleFor(x => x).Must(x => x.CustomerId.HasValue || x.ContactId.HasValue ||
                                  x.LeadId.HasValue || x.OpportunityId.HasValue)
            .WithMessage("Not en az bir CRM kaydıyla ilişkilendirilmelidir.");
    }
}

public sealed class UpdateNoteDtoValidator : AbstractValidator<UpdateNoteDto>
{
    public UpdateNoteDtoValidator() => RuleFor(x => x.Content).RequiredText(5000);
}
