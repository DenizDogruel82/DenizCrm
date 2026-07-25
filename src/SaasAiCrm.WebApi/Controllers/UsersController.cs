using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Users;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize(Roles = "Admin"), Route("api/users")]
public sealed class UsersController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<UserDto>>(new GetUsersQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<UserDto?>(new GetUserByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<UserDto>(new CreateUserCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<UserDto?>(new UpdateUserCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteUserCommand(id), ct) ? NoContent() : NotFound();
}
