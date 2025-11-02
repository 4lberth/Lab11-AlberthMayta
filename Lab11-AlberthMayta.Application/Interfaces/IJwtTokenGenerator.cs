using Lab10_AlberthMayta.Infrastructure;

namespace Lab10_AlberthMayta.Application.Interfaces
{
    // Este es el contrato que tu Handler de Login necesita.
    public interface IJwtTokenGenerator
    {
        string CreateToken(User user, IEnumerable<string> roles);
    }
}