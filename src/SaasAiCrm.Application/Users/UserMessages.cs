using SaasAiCrm.Application.Abstractions.Messaging;
using SaasAiCrm.Application.Common.Dtos;

namespace SaasAiCrm.Application.Users;

public sealed record CreateUserCommand(CreateUserDto User) : ICommand<UserDto>;

public sealed record UpdateUserCommand(Guid Id, UpdateUserDto User)
    : ICommand<UserDto?>;

public sealed record DeleteUserCommand(Guid Id) : ICommand<bool>;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserDto?>;

public sealed record GetUsersQuery : IQuery<IReadOnlyList<UserDto>>;
