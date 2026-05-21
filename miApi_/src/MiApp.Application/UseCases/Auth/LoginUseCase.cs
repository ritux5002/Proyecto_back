using MiApp.Domain.Interfaces;
using BCrypt.Net;

namespace MiApp.Application.UseCases.Auth
{
    public class LoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginUseCase(
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<string?> Execute(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null)
                return null;

            // Verificar contraseña con hash (BCrypt)
            if (!BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash))
                return null;

            return _tokenService.GenerateToken(user);
        }
    }
}
