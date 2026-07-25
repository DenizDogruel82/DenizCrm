using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Tenants;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize(Roles = "Admin"), Route("api/tenants")]
public sealed class TenantsController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<TenantDto?>(new GetTenantByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [AllowAnonymous, HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<TenantDto>> BySlug(string slug, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<TenantDto?>(new GetTenantBySlugQuery(slug), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TenantDto>> Create(CreateTenantDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<TenantDto>(new CreateTenantCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenantDto>> Update(
        Guid id, UpdateTenantDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<TenantDto?>(new UpdateTenantCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
