using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Customers;

public sealed record CreateCustomerCommand(CreateCustomerDto Customer)
    : ICommand<CustomerDto>;

public sealed record UpdateCustomerCommand(Guid Id, UpdateCustomerDto Customer)
    : ICommand<CustomerDto?>;

public sealed record DeleteCustomerCommand(Guid Id) : ICommand<bool>;
