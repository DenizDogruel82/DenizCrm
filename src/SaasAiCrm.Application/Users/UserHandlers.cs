using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Users;

public sealed class UserCommandHandlers(IUserRepository repository, IPasswordService passwords,
    IUnitOfWork unit, ICurrentUser current) : ICommandHandler<CreateUserCommand, UserDto>,
    ICommandHandler<UpdateUserCommand, UserDto?>, ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<UserDto> HandleAsync(CreateUserCommand c, CancellationToken ct = default)
    {
        var d = c.User; var e = new User { TenantId = current.TenantId,
            CreatedByUserId = current.UserId, Email = d.Email.Trim().ToLowerInvariant(),
            FullName = d.FullName, PasswordHash = string.Empty, Role = d.Role };
        e.PasswordHash = passwords.Hash(e, d.Password);
        await repository.AddAsync(e, ct); await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<UserDto?> HandleAsync(UpdateUserCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(current.TenantId, c.Id, ct); if (e is null) return null;
        e.FullName = c.User.FullName; e.Role = c.User.Role; e.IsActive = c.User.IsActive;
        e.UpdatedAtUtc = DateTime.UtcNow; repository.Update(e);
        await unit.SaveChangesAsync(ct); return e.ToDto();
    }
    public async Task<bool> HandleAsync(DeleteUserCommand c, CancellationToken ct = default)
    {
        var e = await repository.GetByIdAsync(current.TenantId, c.Id, ct); if (e is null) return false;
        repository.Remove(e); await unit.SaveChangesAsync(ct); return true;
    }
}

public sealed class UserQueryHandlers(IUserRepository repository, ICurrentUser current)
    : IQueryHandler<GetUserByIdQuery, UserDto?>,
      IQueryHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<UserDto?> HandleAsync(GetUserByIdQuery q, CancellationToken ct = default) =>
        (await repository.GetByIdAsync(current.TenantId, q.Id, ct))?.ToDto();
    public async Task<IReadOnlyList<UserDto>> HandleAsync(GetUsersQuery q,
        CancellationToken ct = default) =>
        (await repository.GetByTenantAsync(current.TenantId, ct)).Select(x => x.ToDto()).ToArray();
}
