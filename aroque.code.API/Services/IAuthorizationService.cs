using aroque.code.API.Models.Custom;

namespace aroque.code.API.Services
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponse> getToken(AuthorizationRequest authorization);
        Task<AuthorizationResponse> getRefreshToken(RefreshTokenRequest refreshToken, int idUsuario);
    }
}
