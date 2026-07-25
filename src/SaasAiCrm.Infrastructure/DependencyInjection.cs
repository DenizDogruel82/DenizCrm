using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Application.Abstractions.Persistence;
using SaasAiCrm.Infrastructure.Authentication;
using SaasAiCrm.Infrastructure.Persistence;

namespace SaasAiCrm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(x =>
                !string.IsNullOrWhiteSpace(x.Issuer) &&
                !string.IsNullOrWhiteSpace(x.Audience),
                "JWT issuer and audience are required.")
            .Validate(x => x.Key.Length >= 32, "JWT key must be at least 32 characters.")
            .ValidateOnStart();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        return services;
    }
}
