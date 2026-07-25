using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Common.Mappings;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Customers;

public sealed class CreateCustomerHandler(ICustomerRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> HandleAsync(CreateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var d = command.Customer;
        var entity = new Customer { TenantId = current.TenantId, CreatedByUserId = current.UserId,
            Name = d.Name, Type = d.Type, Email = d.Email, Phone = d.Phone, Website = d.Website,
            Industry = d.Industry, TaxNumber = d.TaxNumber, Address = d.Address, City = d.City,
            Country = d.Country, OwnerUserId = d.OwnerUserId };
        await repository.AddAsync(entity, cancellationToken);
        await unit.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }
}

public sealed class UpdateCustomerHandler(ICustomerRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<UpdateCustomerCommand, CustomerDto?>
{
    public async Task<CustomerDto?> HandleAsync(UpdateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (entity?.TenantId != current.TenantId) return null;
        var d = command.Customer;
        entity.Name = d.Name; entity.Type = d.Type; entity.Email = d.Email; entity.Phone = d.Phone;
        entity.Website = d.Website; entity.Industry = d.Industry; entity.TaxNumber = d.TaxNumber;
        entity.Address = d.Address; entity.City = d.City; entity.Country = d.Country;
        entity.OwnerUserId = d.OwnerUserId; entity.IsActive = d.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        repository.Update(entity); await unit.SaveChangesAsync(cancellationToken);
        return entity.ToDto();
    }
}

public sealed class DeleteCustomerHandler(ICustomerRepository repository, IUnitOfWork unit,
    ICurrentUser current) : ICommandHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (entity?.TenantId != current.TenantId) return false;
        repository.Remove(entity); await unit.SaveChangesAsync(cancellationToken); return true;
    }
}

public sealed class GetCustomerByIdHandler(ICustomerRepository repository, ICurrentUser current)
    : IQueryHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> HandleAsync(GetCustomerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(query.Id, cancellationToken);
        return entity?.TenantId == current.TenantId ? entity.ToDto() : null;
    }
}

public sealed class GetCustomersHandler(ICustomerRepository repository, ICurrentUser current)
    : IQueryHandler<GetCustomersQuery, PagedResultDto<CustomerDto>>
{
    public async Task<PagedResultDto<CustomerDto>> HandleAsync(GetCustomersQuery query,
        CancellationToken cancellationToken = default)
    {
        var values = string.IsNullOrWhiteSpace(query.Search)
            ? await repository.GetByTenantAsync(current.TenantId, cancellationToken)
            : await repository.SearchAsync(current.TenantId, query.Search, cancellationToken);
        var items = values.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => x.ToDto()).ToArray();
        return new(items, query.PageNumber, query.PageSize, values.Count);
    }
}
