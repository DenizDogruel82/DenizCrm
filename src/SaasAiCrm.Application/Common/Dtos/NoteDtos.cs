namespace SaasAiCrm.Application.Common.Dtos;

public sealed record NoteDto(
    Guid Id,
    string Content,
    Guid? CustomerId,
    Guid? ContactId,
    Guid? LeadId,
    Guid? OpportunityId,
    Guid? CreatedByUserId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record CreateNoteDto(
    string Content,
    Guid? CustomerId,
    Guid? ContactId,
    Guid? LeadId,
    Guid? OpportunityId);

public sealed record UpdateNoteDto(string Content);
