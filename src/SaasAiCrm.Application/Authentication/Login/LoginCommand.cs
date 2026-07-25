namespace SaasAiCrm.Application.Authentication.Login;

public sealed record LoginCommand(string Email, string Password);

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserSummary User);

public sealed record UserSummary(Guid Id, string Email, string FullName, string Role);
