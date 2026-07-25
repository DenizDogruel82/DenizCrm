using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaasAiCrm.Application.Abstractions.Authentication;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    private static readonly Guid DemoTenantId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid DemoAdminId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var passwords = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        await db.Database.MigrateAsync(cancellationToken);

        if (!await db.Tenants.AnyAsync(x => x.Id == DemoTenantId, cancellationToken))
        {
            db.Tenants.Add(new Tenant
            {
                Id = DemoTenantId,
                Name = "SaaS AI CRM Demo",
                Slug = "demo"
            });
        }

        if (!await db.Users.AnyAsync(x => x.Id == DemoAdminId, cancellationToken))
        {
            var admin = new User
            {
                Id = DemoAdminId,
                TenantId = DemoTenantId,
                Email = "admin@saasaicrm.com",
                FullName = "CRM Yöneticisi",
                Role = "Admin",
                PasswordHash = string.Empty
            };
            admin.PasswordHash = passwords.Hash(admin, "Admin123!");
            db.Users.Add(admin);
        }

        if (!await db.PipelineStages.AnyAsync(
                x => x.TenantId == DemoTenantId,
                cancellationToken))
        {
            db.PipelineStages.AddRange(
                Stage("Yeni", 1, 10, "#64748B"),
                Stage("Görüşme", 2, 30, "#3B82F6"),
                Stage("Teklif", 3, 60, "#F59E0B"),
                Stage("Kazanıldı", 4, 100, "#10B981"));
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static PipelineStage Stage(
        string name,
        int order,
        int probability,
        string color) =>
        new()
        {
            TenantId = DemoTenantId,
            Name = name,
            Order = order,
            WinProbability = probability,
            Color = color
        };
}
