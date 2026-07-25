using System.Security.Claims;
using SaasAiCrm.Application.Abstractions.Authentication;

namespace SaasAiCrm.WebApi.Services;

internal sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;
    public Guid UserId => ReadGuid(ClaimTypes.NameIdentifier);
    public Guid TenantId => ReadGuid("tenant_id");

    private Guid ReadGuid(string claimType) =>
        Guid.TryParse(Principal?.FindFirstValue(claimType), out var value)
            ? value
            : throw new UnauthorizedAccessException($"Required claim '{claimType}' is missing.");
}
