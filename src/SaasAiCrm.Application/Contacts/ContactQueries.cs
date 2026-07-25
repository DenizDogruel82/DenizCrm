using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Contacts;

public sealed record GetContactByIdQuery(Guid Id) : IQuery<ContactDto?>;

public sealed record GetContactsByCustomerQuery(Guid CustomerId)
    : IQuery<IReadOnlyList<ContactDto>>;
