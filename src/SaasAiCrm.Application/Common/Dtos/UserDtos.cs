namespace SaasAiCrm.Application.Common.Dtos;

public sealed record UserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string FullName,
    string Role,
    bool IsActive,
    DateTime? LastLoginAtUtc);

public sealed record CreateUserDto(
    string Email,
    string FullName,
    string Password,
    string Role);

public sealed record UpdateUserDto(
    string FullName,
    string Role,
    bool IsActive);
