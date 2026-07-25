using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface IContactRepository : IRepository<Contact>
{
    Task<IReadOnlyList<Contact>> GetByCustomerAsync(
        Guid tenantId,
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<Contact?> GetByEmailAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default);
}
