using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Application.PipelineStages;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController, Authorize, Route("api/pipeline-stages")]
public sealed class PipelineStagesController(IMessageDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PipelineStageDto>>> GetAll(CancellationToken ct) =>
        Ok(await dispatcher.QueryAsync<IReadOnlyList<PipelineStageDto>>(
            new GetPipelineStagesQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PipelineStageDto>> Get(Guid id, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<PipelineStageDto?>(
            new GetPipelineStageByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PipelineStageDto>> Create(
        CreatePipelineStageDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<PipelineStageDto>(
            new CreatePipelineStageCommand(dto), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PipelineStageDto>> Update(
        Guid id, UpdatePipelineStageDto dto, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<PipelineStageDto?>(
            new UpdatePipelineStageCommand(id, dto), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await dispatcher.SendAsync<bool>(new DeletePipelineStageCommand(id), ct)
            ? NoContent() : NotFound();
}
