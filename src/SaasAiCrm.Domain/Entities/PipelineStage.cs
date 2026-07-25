using SaasAiCrm.Domain.Common;

namespace SaasAiCrm.Domain.Entities;

public sealed class PipelineStage : TenantEntity
{
    public required string Name { get; set; }
    public int Order { get; set; }
    public int WinProbability { get; set; }
    public string Color { get; set; } = "#7C5CFF";
    public bool IsActive { get; set; } = true;
}
