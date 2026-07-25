namespace SaasAiCrm.Domain.Enums;

public enum CustomerType
{
    Individual = 1,
    Company = 2
}

public enum LeadStatus
{
    New = 1,
    Contacted = 2,
    Qualified = 3,
    Unqualified = 4,
    Converted = 5,
    Lost = 6
}

public enum OpportunityStatus
{
    Open = 1,
    Won = 2,
    Lost = 3
}

public enum ActivityType
{
    Call = 1,
    Email = 2,
    Meeting = 3,
    Task = 4,
    Demo = 5
}

public enum ActivityStatus
{
    Planned = 1,
    Completed = 2,
    Cancelled = 3
}

public enum AiInsightType
{
    LeadScore = 1,
    ChurnRisk = 2,
    NextBestAction = 3,
    Sentiment = 4,
    OpportunityForecast = 5,
    Summary = 6
}

public enum SubscriptionStatus
{
    Trial = 1,
    Active = 2,
    PastDue = 3,
    Cancelled = 4,
    Expired = 5
}
