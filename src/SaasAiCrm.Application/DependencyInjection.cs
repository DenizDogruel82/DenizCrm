using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SaasAiCrm.Application.Authentication.Login;

namespace SaasAiCrm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        services.AddScoped<LoginHandler>();
        return services;
    }
}
