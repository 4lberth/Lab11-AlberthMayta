using Lab10_AlberthMayta.Domain;
using Lab10_AlberthMayta.Infrastructure;
using Lab11_AlberthMayta.Domain.Ports;

namespace Lab10_AlberthMayta.Domain.Ports
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        // Puedes agregar métodos específicos si los necesitas
        Task<IEnumerable<UserRole>> GetRolesByUserIdAsync(Guid userId);
    }
}