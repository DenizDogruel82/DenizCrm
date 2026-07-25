using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Infrastructure.Persistence.Configurations;

internal abstract class TenantEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Domain.Common.TenantEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
    }
}

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.TimeZone).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}

internal sealed class UserConfiguration : TenantEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        builder.ToTable("Users");
        builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(150).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Role).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

internal sealed class CustomerConfiguration : TenantEntityConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        builder.ToTable("Customers");
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(254);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Website).HasMaxLength(300);
        builder.Property(x => x.Industry).HasMaxLength(100);
        builder.Property(x => x.TaxNumber).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.HasIndex(x => new { x.TenantId, x.Name });
    }
}

internal sealed class ContactConfiguration : TenantEntityConfiguration<Contact>
{
    public override void Configure(EntityTypeBuilder<Contact> builder)
    {
        base.Configure(builder);
        builder.ToTable("Contacts");
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.JobTitle).HasMaxLength(150);
        builder.Property(x => x.Email).HasMaxLength(254);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class LeadConfiguration : TenantEntityConfiguration<Lead>
{
    public override void Configure(EntityTypeBuilder<Lead> builder)
    {
        base.Configure(builder);
        builder.ToTable("Leads");
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CompanyName).HasMaxLength(200);
        builder.Property(x => x.Email).HasMaxLength(254);
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Source).HasMaxLength(100);
        builder.HasIndex(x => new { x.TenantId, x.Status });
    }
}

internal sealed class PipelineStageConfiguration : TenantEntityConfiguration<PipelineStage>
{
    public override void Configure(EntityTypeBuilder<PipelineStage> builder)
    {
        base.Configure(builder);
        builder.ToTable("PipelineStages");
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(7).IsFixedLength().IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.Order }).IsUnique();
    }
}

internal sealed class OpportunityConfiguration : TenantEntityConfiguration<Opportunity>
{
    public override void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        base.Configure(builder);
        builder.ToTable("Opportunities");
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(x => x.LostReason).HasMaxLength(500);
        builder.HasIndex(x => new { x.TenantId, x.PipelineStageId });
        builder.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PipelineStage>().WithMany().HasForeignKey(x => x.PipelineStageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ActivityConfiguration : TenantEntityConfiguration<Activity>
{
    public override void Configure(EntityTypeBuilder<Activity> builder)
    {
        base.Configure(builder);
        builder.ToTable("Activities");
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.HasIndex(x => new { x.TenantId, x.DueAtUtc });
    }
}

internal sealed class NoteConfiguration : TenantEntityConfiguration<Note>
{
    public override void Configure(EntityTypeBuilder<Note> builder)
    {
        base.Configure(builder);
        builder.ToTable("Notes");
        builder.Property(x => x.Content).HasMaxLength(5000).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.CustomerId });
        builder.HasIndex(x => new { x.TenantId, x.OpportunityId });
    }
}

internal sealed class AiInsightConfiguration : TenantEntityConfiguration<AiInsight>
{
    public override void Configure(EntityTypeBuilder<AiInsight> builder)
    {
        base.Configure(builder);
        builder.ToTable("AiInsights");
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Content).HasMaxLength(5000).IsRequired();
        builder.Property(x => x.Score).HasPrecision(9, 4);
        builder.Property(x => x.Confidence).HasPrecision(5, 4);
        builder.Property(x => x.Model).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.Type });
    }
}

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlanCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ExternalCustomerId).HasMaxLength(200);
        builder.Property(x => x.ExternalSubscriptionId).HasMaxLength(200);
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.ExternalSubscriptionId).IsUnique()
            .HasFilter("[ExternalSubscriptionId] IS NOT NULL");
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
