using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.Notes;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/notes")]
public sealed class NotesController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IReadOnlyList<NoteDto>>> ByCustomer(
        Guid customerId, CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<NoteDto>>(
            new GetNotesByCustomerQuery(customerId), ct));

    [HttpGet("opportunity/{opportunityId:guid}")]
    public async Task<ActionResult<IReadOnlyList<NoteDto>>> ByOpportunity(
        Guid opportunityId, CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<NoteDto>>(
            new GetNotesByOpportunityQuery(opportunityId), ct));

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create(CreateNoteDto dto, CancellationToken ct) =>
        Ok(await dispatcher.SendAsync<NoteDto>(new CreateNoteCommand(dto), ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<NoteDto>> Update(Guid id, UpdateNoteDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<NoteDto?>(new UpdateNoteCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeleteNoteCommand(id), ct) ? NoContent() : NotFound();
}
