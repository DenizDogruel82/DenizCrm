using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SaasAiCrm.Application.Authentication.Login;
using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Messaging;

namespace SaasAiCrm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        services.AddScoped<IMessageDispatcher, MessageDispatcher>();

        var assembly = typeof(DependencyInjection).Assembly;
        var handlerDefinitions = new[] { typeof(ICommandHandler<,>), typeof(IQueryHandler<,>) };
        foreach (var implementation in assembly.GetTypes().Where(x => x is { IsAbstract: false, IsInterface: false }))
        {
            foreach (var service in implementation.GetInterfaces().Where(x =>
                         x.IsGenericType &&
                         handlerDefinitions.Contains(x.GetGenericTypeDefinition())))
            {
                services.AddScoped(service, implementation);
            }
        }

        services.AddScoped<LoginHandler>();
        return services;
    }
}
