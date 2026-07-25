using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Notes;

public sealed record CreateNoteCommand(CreateNoteDto Note) : ICommand<NoteDto>;

public sealed record UpdateNoteCommand(Guid Id, UpdateNoteDto Note)
    : ICommand<NoteDto?>;

public sealed record DeleteNoteCommand(Guid Id) : ICommand<bool>;

public sealed record GetNotesByCustomerQuery(Guid CustomerId)
    : IQuery<IReadOnlyList<NoteDto>>;

public sealed record GetNotesByOpportunityQuery(Guid OpportunityId)
    : IQuery<IReadOnlyList<NoteDto>>;
