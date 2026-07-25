namespace SaasAiCrm.Application.Common.Dtos;

public sealed record ContactDto(
    Guid Id,
    Guid CustomerId,
    string FirstName,
    string LastName,
    string? JobTitle,
    string? Email,
    string? Phone,
    bool IsPrimary,
    bool HasEmailConsent,
    DateTime CreatedAtUtc);

public sealed record CreateContactDto(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string? JobTitle,
    string? Email,
    string? Phone,
    bool IsPrimary,
    bool HasEmailConsent);

public sealed record UpdateContactDto(
    string FirstName,
    string LastName,
    string? JobTitle,
    string? Email,
    string? Phone,
    bool IsPrimary,
    bool HasEmailConsent);
