using SaasAiCrm.Domain.Entities;

namespace SaasAiCrm.Application.Abstractions.Authentication;

public interface IPasswordService
{
    string Hash(User user, string password);
    bool Verify(User user, string passwordHash, string password);
}
