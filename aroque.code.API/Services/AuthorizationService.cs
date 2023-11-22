using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using aroque.code.API.Models;
using aroque.code.API.Models.Custom;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace aroque.code.API.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly RedbrowContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthorizationService(RedbrowContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Metodo que nos permite generar nuestro web token
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns>token</returns>
        private string CreateToken(string idUsuario)
        {

            var key = _configuration.GetValue<string>("JwtSettings:key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario));

            var credencialesToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = credencialesToken
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string token = tokenHandler.WriteToken(tokenConfig);

            return token;
        }
        
        /// <summary>
        /// Metodo que nos permite crear un refresh token
        /// </summary>
        /// <returns>refresh token</returns>
        private string CreateRefreshToken()
        {
            var byteArray = new byte[64];
            var refreshToken = "";

            using (var mg = RandomNumberGenerator.Create()) {
                mg.GetBytes(byteArray);
                refreshToken = Convert.ToBase64String(byteArray);
            }

            return refreshToken;
        }
        
        /// <summary>
        /// Nos permtie guardar el historial de los token creados y vigentes
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="token"></param>
        /// <param name="refreshToken"></param>
        /// <returns>AuthorizationResponse</returns>
        private async Task<AuthorizationResponse> SaveHistoryToken(int idUsuario,string token,string refreshToken){

            var tokenHistry = new TokenHistory
            {
                IdUsuario = idUsuario,
                Token = token,
                RefreshToken = refreshToken,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(5)
            };


            await _dbContext.TokenHistories.AddAsync(tokenHistry);
            await _dbContext.SaveChangesAsync();

            return new AuthorizationResponse { Token = token, RefreshToken = refreshToken, Response = true, Message = "Token refresh guardado correctamente" };

        }
        public async Task<AuthorizationResponse> getToken(AuthorizationRequest authorization)
        {
            var user = _dbContext.Usuarios.FirstOrDefault(x => 
                x.Correo == authorization.UserEmail 
                && x.Clave == authorization.UserPassword);

            if (user == null)
            {
                return await Task.FromResult<AuthorizationResponse>(null);
            }

            string token = CreateToken(user.IdUsuario.ToString());
            string refreshToken = CreateRefreshToken();

            return await SaveHistoryToken(user.IdUsuario,token,refreshToken);
        }

        public async Task<AuthorizationResponse> getRefreshToken(RefreshTokenRequest refreshToken, int idUsuario)
        {
            var historyToken = _dbContext.TokenHistories.FirstOrDefault(x =>
            x.IdUsuario == idUsuario
            && x.Token == refreshToken.TokenExpirado
            && x.RefreshToken == refreshToken.RefreshToken);

            if (historyToken == null)
            {
                return new AuthorizationResponse {Response = false, Message = "No se ha encotrado el refresh token"};
            }

            var resfreshToken = CreateRefreshToken();
            var token = CreateToken(idUsuario.ToString());

            return await SaveHistoryToken(idUsuario, token, resfreshToken);
        }
    }
}
