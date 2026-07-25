using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Abstractions.Authentication;

public interface ITokenService
{
    TokenResult Create(User user);
}

public sealed record TokenResult(string AccessToken, DateTime ExpiresAtUtc);
