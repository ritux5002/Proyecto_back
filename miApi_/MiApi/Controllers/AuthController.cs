using Microsoft.AspNetCore.Mvc;
using MiApp.Application.UseCases.Auth;
using MiApi.DTOs;

namespace MiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginUseCase _loginUseCase;

        public AuthController(LoginUseCase loginUseCase)
        {
            _loginUseCase = loginUseCase;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _loginUseCase.Execute(request.Email, request.Password);
            if (result is null)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            return Ok(new { token = result });
        }
    }
}
