using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Contacts;

public sealed class CreateContactHandler(IContactRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<CreateContactCommand, ContactDto>
{
    public async Task<ContactDto> HandleAsync(CreateContactCommand command,
        CancellationToken cancellationToken = default)
    {
        var d = command.Contact;
        var entity = new Contact { TenantId = current.TenantId, CreatedByUserId = current.UserId,
            CustomerId = d.CustomerId, FirstName = d.FirstName, LastName = d.LastName,
            JobTitle = d.JobTitle, Email = d.Email, Phone = d.Phone, IsPrimary = d.IsPrimary,
            HasEmailConsent = d.HasEmailConsent };
        await repository.AddAsync(entity, cancellationToken);
        await unit.SaveChangesAsync(cancellationToken); return entity.ToDto();
    }
}

public sealed class UpdateContactHandler(IContactRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<UpdateContactCommand, ContactDto?>
{
    public async Task<ContactDto?> HandleAsync(UpdateContactCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId) return null;
        var d = command.Contact; e.FirstName = d.FirstName; e.LastName = d.LastName;
        e.JobTitle = d.JobTitle; e.Email = d.Email; e.Phone = d.Phone; e.IsPrimary = d.IsPrimary;
        e.HasEmailConsent = d.HasEmailConsent; e.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(e); await unit.SaveChangesAsync(cancellationToken); return e.ToDto();
    }
}

public sealed class DeleteContactHandler(IContactRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<DeleteContactCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteContactCommand command,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (e?.TenantId != current.TenantId) return false;
        repository.Remove(e); await unit.SaveChangesAsync(cancellationToken); return true;
    }
}

public sealed class GetContactByIdHandler(IContactRepository repository, ICurrentUser current)
    : IQueryHandler<GetContactByIdQuery, ContactDto?>
{
    public async Task<ContactDto?> HandleAsync(GetContactByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var e = await repository.GetByIdAsync(query.Id, cancellationToken);
        return e?.TenantId == current.TenantId ? e.ToDto() : null;
    }
}

public sealed class GetContactsByCustomerHandler(IContactRepository repository,
    ICurrentUser current) : IQueryHandler<GetContactsByCustomerQuery, IReadOnlyList<ContactDto>>
{
    public async Task<IReadOnlyList<ContactDto>> HandleAsync(GetContactsByCustomerQuery query,
        CancellationToken cancellationToken = default) =>
        (await repository.GetByCustomerAsync(current.TenantId, query.CustomerId, cancellationToken))
        .Select(x => x.ToDto()).ToArray();
}
