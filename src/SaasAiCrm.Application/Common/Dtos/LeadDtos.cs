using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Common.Dtos;

public sealed record LeadDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Source,
    LeadStatus Status,
    int Score,
    Guid? OwnerUserId,
    Guid? ConvertedCustomerId,
    Guid? ConvertedContactId,
    DateTime? ConvertedAtUtc,
    DateTime CreatedAtUtc);

public sealed record CreateLeadDto(
    string FirstName,
    string LastName,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Source,
    Guid? OwnerUserId);

public sealed record UpdateLeadDto(
    string FirstName,
    string LastName,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Source,
    LeadStatus Status,
    int Score,
    Guid? OwnerUserId);
