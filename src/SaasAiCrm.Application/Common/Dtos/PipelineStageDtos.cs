namespace SaasAiCrm.Application.Common.Dtos;

public sealed record PipelineStageDto(
    Guid Id,
    string Name,
    int Order,
    int WinProbability,
    string Color,
    bool IsActive);

public sealed record CreatePipelineStageDto(
    string Name,
    int Order,
    int WinProbability,
    string Color);

public sealed record UpdatePipelineStageDto(
    string Name,
    int Order,
    int WinProbability,
    string Color,
    bool IsActive);
