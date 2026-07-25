using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Contacts;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/contacts")]
public sealed class ContactsController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<ContactDto?>(new GetContactByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ContactDto>>> ByCustomer(
        Guid customerId, CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<ContactDto>>(
            new GetContactsByCustomerQuery(customerId), ct));

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create(CreateContactDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ContactDto>(new CreateContactCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContactDto>> Update(
        Guid id, UpdateContactDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ContactDto?>(new UpdateContactCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteContactCommand(id), ct) ? NoContent() : NotFound();
}
