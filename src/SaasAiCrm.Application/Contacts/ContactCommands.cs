using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Contacts;

public sealed record CreateContactCommand(CreateContactDto Contact)
    : ICommand<ContactDto>;

public sealed record UpdateContactCommand(Guid Id, UpdateContactDto Contact)
    : ICommand<ContactDto?>;

public sealed record DeleteContactCommand(Guid Id) : ICommand<bool>;
