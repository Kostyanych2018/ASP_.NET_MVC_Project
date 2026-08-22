namespace GameStore.Application.Common.Interfaces;

public interface IUserContext
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}