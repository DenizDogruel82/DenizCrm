using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
