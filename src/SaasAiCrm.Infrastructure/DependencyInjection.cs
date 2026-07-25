using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<CrmDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(CrmDbContext).Assembly.FullName)
                    .EnableRetryOnFailure()));
        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<CrmDbContext>());
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IOpportunityRepository, OpportunityRepository>();
        services.AddScoped<IPipelineStageRepository, PipelineStageRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IAiInsightRepository, AiInsightRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(x =>
                !string.IsNullOrWhiteSpace(x.Issuer) &&
                !string.IsNullOrWhiteSpace(x.Audience),
                "JWT issuer and audience are required.")
            .Validate(x => x.Key.Length >= 32, "JWT key must be at least 32 characters.")
            .ValidateOnStart();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        return services;
    }
}
