using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Leads;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/leads")]
public sealed class LeadsController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<LeadDto>>> GetAll(
        [FromQuery] LeadStatus? status, [FromQuery] Guid? ownerUserId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await dispatcher.QueryAsync<PagedResultDto<LeadDto>>(
            new GetLeadsQuery(status, ownerUserId, pageNumber, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeadDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<LeadDto?>(new GetLeadByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LeadDto>> Create(CreateLeadDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<LeadDto>(new CreateLeadCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LeadDto>> Update(Guid id, UpdateLeadDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<LeadDto?>(new UpdateLeadCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/convert")]
    public async Task<ActionResult<LeadDto>> Convert(
        Guid id, ConvertLeadRequest request, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<LeadDto?>(
            new ConvertLeadCommand(id, request.CustomerId, request.ContactId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteLeadCommand(id), ct) ? NoContent() : NotFound();
}

public sealed record ConvertLeadRequest(Guid CustomerId, Guid ContactId);
