using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.AiInsights;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/ai-insights")]
public sealed class AiInsightsController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AiInsightDto>>> GetAll(
        [FromQuery] AiInsightType? type, CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<AiInsightDto>>(
            new GetAiInsightsQuery(type), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AiInsightDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<AiInsightDto?>(new GetAiInsightByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("lead/{leadId:guid}")]
    public async Task<ActionResult<IReadOnlyList<AiInsightDto>>> ByLead(
        Guid leadId, CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<AiInsightDto>>(
            new GetLeadAiInsightsQuery(leadId), ct));

    [HttpGet("opportunity/{opportunityId:guid}")]
    public async Task<ActionResult<IReadOnlyList<AiInsightDto>>> ByOpportunity(
        Guid opportunityId, CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<AiInsightDto>>(
            new GetOpportunityAiInsightsQuery(opportunityId), ct));

    [HttpPost]
    public async Task<ActionResult<AiInsightDto>> Create(
        CreateAiInsightDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<AiInsightDto>(
            new CreateAiInsightCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/dismiss")]
    public async Task<ActionResult<AiInsightDto>> Dismiss(
        Guid id, DismissAiInsightDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<AiInsightDto?>(
            new DismissAiInsightCommand(id, dto.IsDismissed), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteAiInsightCommand(id), ct)
            ? NoContent() : NotFound();
}
