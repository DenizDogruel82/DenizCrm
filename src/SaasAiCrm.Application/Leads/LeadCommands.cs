using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Leads;

public sealed record CreateLeadCommand(CreateLeadDto Lead) : ICommand<LeadDto>;

public sealed record UpdateLeadCommand(Guid Id, UpdateLeadDto Lead)
    : ICommand<LeadDto?>;

public sealed record DeleteLeadCommand(Guid Id) : ICommand<bool>;

public sealed record ConvertLeadCommand(Guid Id, Guid CustomerId, Guid ContactId)
    : ICommand<LeadDto?>;
