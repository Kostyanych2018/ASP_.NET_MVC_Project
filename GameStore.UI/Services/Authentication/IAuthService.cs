using GameStore.UI.Models;

namespace GameStore.UI.Services.Authentication;

public interface IAuthService
{
    Task RegisterUserAsync(RegisterUserViewModel model, CancellationToken cancellationToken = default);
}