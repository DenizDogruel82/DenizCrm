namespace SaasAiCrm.Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);
}

public static class CacheKeys
{
    public static string Tenant(Guid tenantId) => $"tenant:{tenantId}";
    public static string PipelineStages(Guid tenantId) => $"tenant:{tenantId}:pipeline-stages";
    public static string Subscription(Guid tenantId) => $"tenant:{tenantId}:subscription";
    public static string User(Guid tenantId, Guid userId) => $"tenant:{tenantId}:user:{userId}";
}
