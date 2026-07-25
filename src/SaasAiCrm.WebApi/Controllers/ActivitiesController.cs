using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Activities;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Domain.Enums;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/activities")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public sealed class ActivitiesController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ActivityDto>>> GetAll(
        [FromQuery] ActivityStatus? status, [FromQuery] Guid? assignedUserId,
        [FromQuery] DateTime? untilUtc, CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<ActivityDto>>(
            new GetActivitiesQuery(status, assignedUserId, untilUtc), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ActivityDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<ActivityDto?>(new GetActivityByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ActivityDto>> Create(CreateActivityDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ActivityDto>(new CreateActivityCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ActivityDto>> Update(
        Guid id, UpdateActivityDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ActivityDto?>(
            new UpdateActivityCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<ActivityDto>> Complete(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ActivityDto?>(
            new CompleteActivityCommand(id, DateTime.UtcNow), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteActivityCommand(id), ct)
            ? NoContent() : NotFound();
}
