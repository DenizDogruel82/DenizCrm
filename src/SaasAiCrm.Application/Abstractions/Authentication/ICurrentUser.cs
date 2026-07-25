namespace SaasAiCrm.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid TenantId { get; }
    bool IsAuthenticated { get; }
}
