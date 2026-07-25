using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SaasAiCrm.Application.Abstractions.Ai;

namespace SaasAiCrm.Infrastructure.Ai;

internal sealed class GeminiService(
    HttpClient httpClient,
    IOptions<GeminiOptions> options) : IGenerativeAiService
{
    private readonly GeminiOptions _options = options.Value;

    public async Task<AiGenerationResult> GenerateAsync(
        string prompt,
        string? context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "Gemini API anahtarı yapılandırılmamış. Gemini:ApiKey ayarını User Secrets ile ekleyin.");
        }

        var request = new GeminiRequest(
            new GeminiSystemInstruction([
                new GeminiPart(
                    "Sen Deniz CRM için çalışan Türkçe bir satış ve müşteri ilişkileri asistanısın. " +
                    "Yanıtların kısa, uygulanabilir ve verilen CRM bağlamına sadık olsun.")
            ]),
            [
                new GeminiContent("user", [
                    new GeminiPart(string.IsNullOrWhiteSpace(context)
                        ? prompt
                        : $"CRM bağlamı:\n{context}\n\nKullanıcı isteği:\n{prompt}")
                ])
            ]);

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            $"models/{Uri.EscapeDataString(_options.Model)}:generateContent");
        message.Headers.Add("x-goog-api-key", _options.ApiKey);
        message.Content = JsonContent.Create(request);

        using var response = await httpClient.SendAsync(message, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var detail = TryGetError(json) ?? response.ReasonPhrase ?? "Bilinmeyen Gemini API hatası";
            throw new HttpRequestException($"Gemini isteği başarısız: {detail}", null, response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<GeminiResponse>(
            json,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var text = result?.Candidates?
            .SelectMany(candidate => candidate.Content?.Parts ?? [])
            .Select(part => part.Text)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Gemini yanıtında metin bulunamadı.");
        }

        return new AiGenerationResult(
            text,
            result?.ModelVersion ?? _options.Model,
            result?.UsageMetadata?.PromptTokenCount,
            result?.UsageMetadata?.CandidatesTokenCount);
    }

    private static string? TryGetError(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.GetProperty("error").GetProperty("message").GetString();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private sealed record GeminiRequest(
        GeminiSystemInstruction SystemInstruction,
        GeminiContent[] Contents);
    private sealed record GeminiSystemInstruction(GeminiPart[] Parts);
    private sealed record GeminiContent(string Role, GeminiPart[] Parts);
    private sealed record GeminiPart(string Text);
    private sealed record GeminiResponse(
        GeminiCandidate[]? Candidates,
        GeminiUsageMetadata? UsageMetadata,
        string? ModelVersion);
    private sealed record GeminiCandidate(GeminiContent? Content);
    private sealed record GeminiUsageMetadata(
        int? PromptTokenCount,
        int? CandidatesTokenCount);
}
