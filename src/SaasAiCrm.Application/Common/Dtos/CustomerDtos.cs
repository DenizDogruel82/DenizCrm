using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.Application.Common.Dtos;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    CustomerType Type,
    string? Email,
    string? Phone,
    string? Website,
    string? Industry,
    string? TaxNumber,
    string? Address,
    string? City,
    string? Country,
    Guid? OwnerUserId,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record CreateCustomerDto(
    string Name,
    CustomerType Type,
    string? Email,
    string? Phone,
    string? Website,
    string? Industry,
    string? TaxNumber,
    string? Address,
    string? City,
    string? Country,
    Guid? OwnerUserId);

public sealed record UpdateCustomerDto(
    string Name,
    CustomerType Type,
    string? Email,
    string? Phone,
    string? Website,
    string? Industry,
    string? TaxNumber,
    string? Address,
    string? City,
    string? Country,
    Guid? OwnerUserId,
    bool IsActive);
