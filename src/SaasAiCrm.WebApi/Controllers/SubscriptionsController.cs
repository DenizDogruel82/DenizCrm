using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Subscriptions;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize(Roles = "Admin"), Route("api/subscriptions")]
public sealed class SubscriptionsController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet("current")]
    public async Task<ActionResult<SubscriptionDto>> Current(CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<SubscriptionDto?>(
            new GetCurrentSubscriptionQuery(), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create(
        CreateSubscriptionDto dto, CancellationToken ct) =>
        Ok(await dispatcher.SendAsync<SubscriptionDto>(
            new CreateSubscriptionCommand(dto), ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SubscriptionDto>> Update(
        Guid id, UpdateSubscriptionDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<SubscriptionDto?>(
            new UpdateSubscriptionCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<SubscriptionDto>> Cancel(
        Guid id, [FromQuery] bool atPeriodEnd = true, CancellationToken ct = default)
    {
        var result = await dispatcher.SendAsync<SubscriptionDto?>(
            new CancelSubscriptionCommand(id, atPeriodEnd), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
