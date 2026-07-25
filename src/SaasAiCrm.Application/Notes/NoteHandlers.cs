using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Notes;

public sealed class NoteCommandHandlers(INoteRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<CreateNoteCommand, NoteDto>,
    ICommandHandler<UpdateNoteCommand, NoteDto?>, ICommandHandler<DeleteNoteCommand, bool>
{
    public async Task<NoteDto> HandleAsync(CreateNoteCommand c, CancellationToken ct = default)
    {
        var d = c.Note; var e = new Note { TenantId = current.TenantId,
            CreatedByUserId = current.UserId, Content = d.Content, CustomerId = d.CustomerId,
            ContactId = d.ContactId, LeadId = d.LeadId, OpportunityId = d.OpportunityId };
        await repository.AddAsync(e, ct); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<NoteDto?> HandleAsync(UpdateNoteCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return null;
        e.Content = c.Note.Content; e.UpdatedAtUtc = DateTime.UtcNow; repository.Update(e);
        await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<bool> HandleAsync(DeleteNoteCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(c.Id, ct); if (e?.TenantId != current.TenantId) return false;
        repository.Remove(e); await unit.SaveChangesAsync(ct); return true;
    }
}

public sealed class NoteQueryHandlers(INoteRepository repository, ICurrentUser current)
    : IQueryHandler<GetNotesByCustomerQuery, IReadOnlyList<NoteDto>>,
      IQueryHandler<GetNotesByOpportunityQuery, IReadOnlyList<NoteDto>>
{
    public async Task<IReadOnlyList<NoteDto>> HandleAsync(GetNotesByCustomerQuery q,
        CancellationToken ct = default) => (await repository.GetByCustomerAsync(
            current.TenantId, q.CustomerId, ct)).Select(x => x.ToDto()).ToArray();
    public async Task<IReadOnlyList<NoteDto>> HandleAsync(GetNotesByOpportunityQuery q,
        CancellationToken ct = default) => (await repository.GetByOpportunityAsync(
            current.TenantId, q.OpportunityId, ct)).Select(x => x.ToDto()).ToArray();
}
