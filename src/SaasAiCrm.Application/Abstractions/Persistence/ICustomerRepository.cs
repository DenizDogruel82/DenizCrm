using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Abstractions.Persistence;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IReadOnlyList<Customer>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> SearchAsync(
        Guid tenantId,
        string searchTerm,
        CancellationToken cancellationToken = default);
}
