using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi_otica.Service.Autenticacao;
using WebApi_otica.ViewModels;

namespace WebApi_otica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAutenticacao _autenticacao;
        public AccountController(IConfiguration configuration, IAutenticacao autenticacao)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
            _autenticacao = autenticacao ??
                throw new ArgumentNullException(nameof(autenticacao));

        }

        [HttpPost("CriarUsuario")]
        public async Task<ActionResult<UserToken>> CriarUsuario([FromBody] RegisterModel register)
        {
            if (register.Password != register.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "as senhas não conferem");
                return BadRequest(ModelState);
            }
            var result = await _autenticacao.RegisterUser(register.Email, register.Password);
            if (result)
            {
                return Ok($"Usuario {register.Email} Criado com sucesso");
            }
            else
            {
                ModelState.AddModelError("CriarUsuario", "Registro invalido");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("LoginUsuario")]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginModel userInfo)
        {
            var result = await _autenticacao.Authenticate(userInfo.Email, userInfo.Password);
            if (result)
            {
                return GenerateToken(userInfo);
            }
            else
            {
                ModelState.AddModelError("LoginUsuario", "Login invalido");
                return BadRequest(ModelState);
            }


        }

        private ActionResult<UserToken> GenerateToken(LoginModel userInfo)
        {
            var claims = new[]
            {
                new Claim("email", userInfo.Email),
                new Claim("meuToken", "token do diego"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(40);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
            };
        }
    }
}
