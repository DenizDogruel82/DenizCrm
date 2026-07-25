using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Customers;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/customers")]
public sealed class CustomersController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CustomerDto>>> GetAll(
        [FromQuery] string? search, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await dispatcher.QueryAsync<PagedResultDto<CustomerDto>>(
            new GetCustomersQuery(search, pageNumber, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<CustomerDto?>(new GetCustomerByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CustomerDto>(new CreateCustomerCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(
        Guid id, UpdateCustomerDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CustomerDto?>(new UpdateCustomerCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteCustomerCommand(id), ct) ? NoContent() : NotFound();
}
