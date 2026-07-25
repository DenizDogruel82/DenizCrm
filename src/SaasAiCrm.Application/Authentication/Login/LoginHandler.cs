using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Abstractions.Persistence;

namespace SaasAiCrm.Application.Authentication.Login;

public sealed class LoginHandler(
    IUserRepository users,
    IPasswordService passwords,
    ITokenService tokens) : ICommandHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive ||
            !passwords.Verify(user, user.PasswordHash, command.Password))
        {
            return null;
        }

        var token = tokens.Create(user);
        return new LoginResponse(
            token.AccessToken,
            token.ExpiresAtUtc,
            new UserSummary(user.Id, user.Email, user.FullName, user.Role));
    }
}
