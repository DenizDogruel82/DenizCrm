namespace SaasAiCrm.Infrastructure.Ai;

public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "gemini-3.6-flash";
    public string BaseUrl { get; init; } = "https://generativelanguage.googleapis.com/v1beta/";
}
