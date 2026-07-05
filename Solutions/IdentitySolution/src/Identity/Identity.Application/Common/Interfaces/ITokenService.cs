using Identity.Domain.Entities;

namespace Identity.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid accountId, Profile profile);
    string GenerateAccountOnlyToken(Guid accountId);
}
