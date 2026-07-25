using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.PipelineStages;

public sealed record CreatePipelineStageCommand(CreatePipelineStageDto Stage)
    : ICommand<PipelineStageDto>;

public sealed record UpdatePipelineStageCommand(Guid Id, UpdatePipelineStageDto Stage)
    : ICommand<PipelineStageDto?>;

public sealed record DeletePipelineStageCommand(Guid Id) : ICommand<bool>;

public sealed record GetPipelineStageByIdQuery(Guid Id) : IQuery<PipelineStageDto?>;

public sealed record GetPipelineStagesQuery : IQuery<IReadOnlyList<PipelineStageDto>>;
