using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Customers;

public sealed record GetCustomerByIdQuery(Guid Id) : IQuery<CustomerDto?>;

public sealed record GetCustomersQuery(
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20) : IQuery<PagedResultDto<CustomerDto>>;
