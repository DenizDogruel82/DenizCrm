namespace SaasAiCrm.Application.Abstractions.Ai;

public interface IGenerativeAiService
{
    Task<AiGenerationResult> GenerateAsync(
        string prompt,
        string? context,
        CancellationToken cancellationToken = default);
}

public sealed record AiGenerationResult(
    string Text,
    string Model,
    int? PromptTokenCount,
    int? CandidateTokenCount);
