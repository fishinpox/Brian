namespace Chat.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? ProfileId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
}
