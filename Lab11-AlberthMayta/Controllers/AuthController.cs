using MediatR; // <-- 1. Cambia la inyección
using Lab10_AlberthMayta.Application.Features.Users.Commands.RegisterUser;
using Lab10_AlberthMayta.Application.Features.Users.Queries.LoginUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab10_AlberthMayta.Persistence.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator; // <-- 2. Inyecta IMediator

        public AuthController(IMediator mediator) // <-- 3. Cambia el constructor
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous] // Permite acceso sin autenticación
        public async Task<IActionResult> Register(RegisterUserCommand command) // <-- 4. Recibe el Comando
        {
            try
            {
                var userDto = await _mediator.Send(command); // <-- 5. Envía el Comando
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous] // Permite acceso sin autenticación
        public async Task<IActionResult> Login(LoginUserQuery query) // <-- 4. Recibe la Consulta
        {
            try
            {
                var authResponse = await _mediator.Send(query); // <-- 5. Envía la Consulta
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}