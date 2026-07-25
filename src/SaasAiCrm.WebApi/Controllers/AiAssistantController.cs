using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Ai;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/ai-assistant")]
public sealed class AiAssistantController(IGenerativeAiService aiService) : ControllerBase
{
    [HttpPost("generate")]
    public async Task<ActionResult<AiGenerationResult>> Generate(
        GenerateAiRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    [nameof(request.Prompt)] = ["İstek metni zorunludur."]
                }));
        }

        if (request.Prompt.Length > 4_000 || request.Context?.Length > 12_000)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "AI isteği çok uzun.");
        }

        try
        {
            return Ok(await aiService.GenerateAsync(
                request.Prompt.Trim(),
                request.Context?.Trim(),
                cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Gemini servisi kullanıma hazır değil.",
                detail: exception.Message);
        }
        catch (HttpRequestException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Gemini servisine ulaşılamadı.",
                detail: exception.Message);
        }
    }
}

public sealed record GenerateAiRequest(string Prompt, string? Context);
