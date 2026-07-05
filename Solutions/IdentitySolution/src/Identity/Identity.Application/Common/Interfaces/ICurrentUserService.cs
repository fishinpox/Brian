namespace Identity.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? AccountId { get; }
    Guid? ProfileId { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
}
