namespace SaasAiCrm.Application.Common.Dtos;

public sealed record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    string? LogoUrl,
    string TimeZone,
    string Currency,
    bool IsActive);

public sealed record CreateTenantDto(
    string Name,
    string Slug,
    string? LogoUrl,
    string TimeZone = "Europe/Istanbul",
    string Currency = "TRY");

public sealed record UpdateTenantDto(
    string Name,
    string? LogoUrl,
    string TimeZone,
    string Currency,
    bool IsActive);
