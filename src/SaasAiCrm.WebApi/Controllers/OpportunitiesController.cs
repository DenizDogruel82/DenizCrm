using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Opportunities;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/opportunities")]
public sealed class OpportunitiesController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<OpportunityDto>>> GetAll(
        [FromQuery] Guid? pipelineStageId, [FromQuery] OpportunityStatus? status,
        [FromQuery] Guid? customerId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await dispatcher.QueryAsync<PagedResultDto<OpportunityDto>>(
            new GetOpportunitiesQuery(pipelineStageId, status, customerId, pageNumber, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OpportunityDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<OpportunityDto?>(new GetOpportunityByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OpportunityDto>> Create(
        CreateOpportunityDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<OpportunityDto>(
            new CreateOpportunityCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OpportunityDto>> Update(
        Guid id, UpdateOpportunityDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<OpportunityDto?>(
            new UpdateOpportunityCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:guid}/stage/{stageId:guid}")]
    public async Task<ActionResult<OpportunityDto>> ChangeStage(
        Guid id, Guid stageId, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<OpportunityDto?>(
            new ChangeOpportunityStageCommand(id, stageId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteOpportunityCommand(id), ct)
            ? NoContent() : NotFound();
}
