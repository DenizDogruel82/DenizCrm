using SaasAiCrm.Application.Abstractions.Messaging;

namespace SaasAiCrm.UnitTests.Architecture;

public sealed class CommandQueryHandlerTests
{
    [Fact]
    public void Every_command_and_query_has_a_handler()
    {
        var assembly = typeof(ICommand<>).Assembly;
        var types = assembly.GetTypes();
        var messages = types.Where(type => !type.IsAbstract && !type.IsInterface)
            .Select(type => new
            {
                Message = type,
                Contract = type.GetInterfaces().FirstOrDefault(x =>
                    x.IsGenericType &&
                    (x.GetGenericTypeDefinition() == typeof(ICommand<>) ||
                     x.GetGenericTypeDefinition() == typeof(IQuery<>)))
            })
            .Where(x => x.Contract is not null)
            .ToArray();

        var missing = messages.Where(message =>
        {
            var result = message.Contract!.GetGenericArguments()[0];
            var handlerDefinition = message.Contract.GetGenericTypeDefinition() == typeof(ICommand<>)
                ? typeof(ICommandHandler<,>)
                : typeof(IQueryHandler<,>);
            var handler = handlerDefinition.MakeGenericType(message.Message, result);
            return !types.Any(type => !type.IsAbstract && handler.IsAssignableFrom(type));
        }).Select(x => x.Message.Name).ToArray();

        Assert.True(missing.Length == 0,
            $"Handler bulunamayan mesajlar: {string.Join(", ", missing)}");
    }
}
