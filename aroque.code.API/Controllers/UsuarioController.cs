using aroque.code.API.Models;
using aroque.code.API.Models.Custom;
using aroque.code.response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace aroque.code.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly RedbrowContext _dbContext;
        private readonly aroque.code.API.Services.IAuthorizationService _authorizationService;

        public UsuarioController(RedbrowContext dbContext, aroque.code.API.Services.IAuthorizationService service)
        {
            _dbContext = dbContext;
            _authorizationService = service;
        }

        /// <summary>
        /// Metodo que nos permite generar el token de acceso
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("authorization")]
        public async Task<IActionResult> authorization([FromBody] AuthorizationRequest authorization)
        {
            var response = await _authorizationService.getToken(authorization);

            if (response == null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }

        /// <summary>
        /// Metodo que nos permite actualizar un token en caso que ya este vencido
        /// </summary>
        /// <param name="Tokenrequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("authorizationRefresh")]
        public async Task<IActionResult> authorizationRefresh([FromBody] RefreshTokenRequest Tokenrequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenExpirado = tokenHandler.ReadJwtToken(Tokenrequest.TokenExpirado);

            if (tokenExpirado.ValidTo > DateTime.UtcNow)
            {
                return BadRequest(new AuthorizationResponse { Response = false, Message = "Token aun vigente" });
            }

            string idUsuario = tokenExpirado.Claims.First(x =>
            x.Type == JwtRegisteredClaimNames.NameId).Value.ToString();

            var authorizationResponse = await _authorizationService.getRefreshToken(Tokenrequest, int.Parse(idUsuario));
            if (authorizationResponse.Response)
            {
                return Ok(authorizationResponse);
            }
            else
            {
                return BadRequest(authorizationResponse);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ListUsers")]
        public async Task<IActionResult> listUsers()
        {
            if (_dbContext.Usuarios is null || _dbContext.Usuarios.Count() < 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(await _dbContext.Usuarios.ToListAsync());
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ListUsers/{page}")]
        public async Task<IActionResult> listUsersPagination(int page)
        {
            if (_dbContext.Usuarios == null)
            {
                return NotFound();
            }

            var pageResults = 3f;
            var pageCount = Math.Ceiling(_dbContext.Usuarios.Count() / pageResults);

            var usuarios = await _dbContext.Usuarios
                .Skip((page - 1) * (int)pageResults)
                .Take((int)pageResults)
                .ToListAsync();

            var resonse = new UsuarioResponse
            {
                Usuarios = usuarios,
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return Ok(resonse);
        }

        [Authorize]
        [HttpPost]
        [Route("CreateUsers")]
        public async Task<IActionResult> saveUser(UsuarioDTO user)
        {

            var response = new ResponseAPI<Usuario>();
            var validate = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.Nombre == user.Nombre && x.Correo == user.Correo);
            if (validate is null)
            {
                var usuario = new Usuario
                {
                    Nombre = user.Nombre,
                    Correo = user.Correo,
                    Edad = user.Edad,
                    Clave = user.Clave,
                    CreadoPor = "Web API",
                    FechaCreacion = DateTime.UtcNow
                };

                _dbContext.Usuarios.Add(usuario);
                var result = await _dbContext.SaveChangesAsync();

                response.status = true;
                response.response = usuario;
                response.message = $"usuario creado correctamente";

                return Ok(response);
            }
            else
            {
                response.status = false;
                response.message = $"usuario con el nombre: {user.Nombre} o el correo: {user.Correo} ya existen.";

                return BadRequest(response);
            }

        }

        [Authorize]
        [HttpPut]
        [Route("UpdateUser/{idUser}")]
        public async Task<IActionResult> updateUser(UsuarioDTO user, int idUser)
        {

            var response = new ResponseAPI<Usuario>();
            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUser);
            if (usuario is null)
            {
                response.status = false;
                response.message = "El usuario que se intenta actualizar no existe";

                return BadRequest(response);
            }
            else
            {
                usuario.Nombre = user.Nombre;
                usuario.Correo = user.Correo;
                usuario.Edad = user.Edad;
                usuario.Clave = user.Clave;

                _dbContext.Usuarios.Update(usuario);
                var result = await _dbContext.SaveChangesAsync();

                response.status = true;
                response.response = usuario;
                response.message = $"usuario actualizado correctamente";

                return Ok(response);
            }

        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteUser/{idUser}")]
        public async Task<IActionResult> deleteUser(int idUser)
        {

            var response = new ResponseAPI<Usuario>();
            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUser);
            if (usuario is null)
            {
                response.status = false;
                response.message = "El usuario que se intenta eliminar no existe";

                return BadRequest(response);
            }
            else
            {

                _dbContext.Usuarios.Remove(usuario);
                var result = await _dbContext.SaveChangesAsync();

                response.status = true;
                response.message = "usuario eliminado correctamente";

                return Ok(response);
            }

        }

    }
}
