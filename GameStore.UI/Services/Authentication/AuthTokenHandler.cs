using System.Net.Http.Headers;

namespace GameStore.UI.Services.Authentication;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly ITokenAccessor _tokenAccessor;

    public AuthTokenHandler(ITokenAccessor tokenAccessor)
    {
        _tokenAccessor = tokenAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenAccessor.GetAccessTokenAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}