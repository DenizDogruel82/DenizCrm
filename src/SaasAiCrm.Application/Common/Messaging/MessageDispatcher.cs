using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SaasAiCrm.Application.Abstractions.Messaging;

namespace SaasAiCrm.Application.Common.Messaging;

internal sealed class MessageDispatcher(IServiceProvider services) : IMessageDispatcher
{
    public async Task<TResult> SendAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(command, cancellationToken);
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        dynamic handler = services.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)command, cancellationToken);
    }

    public async Task<TResult> QueryAsync<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(query, cancellationToken);
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic handler = services.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }

    private async Task ValidateAsync<T>(
        T message,
        CancellationToken cancellationToken)
    {
        var validators = services.GetServices<IValidator<T>>();
        var context = new ValidationContext<T>(message);
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
    }
}
