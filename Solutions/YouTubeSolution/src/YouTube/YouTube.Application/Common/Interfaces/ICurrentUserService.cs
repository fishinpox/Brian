namespace YouTube.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? ProfileId { get; }
    bool IsAuthenticated { get; }
}
