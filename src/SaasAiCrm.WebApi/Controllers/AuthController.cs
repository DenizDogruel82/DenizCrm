using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Authentication.Login;

namespace SaasAiCrm.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMessageDispatcher dispatcher) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await dispatcher.SendAsync<LoginResponse?>(command, cancellationToken);
        return response is null
            ? Problem(statusCode: StatusCodes.Status401Unauthorized,
                title: "Giriş başarısız", detail: "E-posta veya parola hatalı.")
            : Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        id = User.FindFirstValue(ClaimTypes.NameIdentifier),
        tenantId = User.FindFirstValue("tenant_id"),
        email = User.FindFirstValue(ClaimTypes.Email),
        fullName = User.Identity?.Name,
        role = User.FindFirstValue(ClaimTypes.Role)
    });
}
