using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
