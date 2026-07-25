using SaasAiCrm.Application.Common.Dtos;
using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Common.Mappings;

internal static class DtoMappings
{
    public static CustomerDto ToDto(this Customer x) => new(x.Id, x.Name, x.Type, x.Email,
        x.Phone, x.Website, x.Industry, x.TaxNumber, x.Address, x.City, x.Country,
        x.OwnerUserId, x.IsActive, x.CreatedAtUtc);
    public static ContactDto ToDto(this Contact x) => new(x.Id, x.CustomerId, x.FirstName,
        x.LastName, x.JobTitle, x.Email, x.Phone, x.IsPrimary, x.HasEmailConsent, x.CreatedAtUtc);
    public static LeadDto ToDto(this Lead x) => new(x.Id, x.FirstName, x.LastName, x.CompanyName,
        x.Email, x.Phone, x.Source, x.Status, x.Score, x.OwnerUserId, x.ConvertedCustomerId,
        x.ConvertedContactId, x.ConvertedAtUtc, x.CreatedAtUtc);
    public static OpportunityDto ToDto(this Opportunity x) => new(x.Id, x.Title, x.CustomerId,
        x.ContactId, x.PipelineStageId, x.OwnerUserId, x.Amount, x.Currency, x.Status,
        x.Probability, x.ExpectedCloseDate, x.LostReason, x.ClosedAtUtc, x.CreatedAtUtc);
    public static PipelineStageDto ToDto(this PipelineStage x) => new(x.Id, x.Name, x.Order,
        x.WinProbability, x.Color, x.IsActive);
    public static ActivityDto ToDto(this Activity x) => new(x.Id, x.Subject, x.Description, x.Type,
        x.Status, x.CustomerId, x.ContactId, x.LeadId, x.OpportunityId, x.AssignedUserId,
        x.DueAtUtc, x.CompletedAtUtc, x.CreatedAtUtc);
    public static NoteDto ToDto(this Note x) => new(x.Id, x.Content, x.CustomerId, x.ContactId,
        x.LeadId, x.OpportunityId, x.CreatedByUserId, x.CreatedAtUtc, x.UpdatedAtUtc);
    public static AiInsightDto ToDto(this AiInsight x) => new(x.Id, x.Type, x.Title, x.Content,
        x.Score, x.Confidence, x.CustomerId, x.LeadId, x.OpportunityId, x.Model,
        x.GeneratedAtUtc, x.ExpiresAtUtc, x.IsDismissed);
    public static TenantDto ToDto(this Tenant x) => new(x.Id, x.Name, x.Slug, x.LogoUrl,
        x.TimeZone, x.Currency, x.IsActive);
    public static UserDto ToDto(this User x) => new(x.Id, x.TenantId, x.Email, x.FullName,
        x.Role, x.IsActive, x.LastLoginAtUtc);
    public static SubscriptionDto ToDto(this Subscription x) => new(x.Id, x.TenantId, x.PlanCode,
        x.Status, x.SeatLimit, x.PeriodStartUtc, x.PeriodEndUtc, x.CancelAtPeriodEnd);
}
