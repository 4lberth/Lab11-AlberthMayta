using MediatR;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Application.Interfaces;
using Lab10_AlberthMayta.Domain.Ports;

namespace Lab10_AlberthMayta.Application.Features.Users.Queries.LoginUser
{
    // 1. La Consulta (DTO de Petición)
    public record LoginUserQuery : IRequest<AuthResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // 2. El Handler (La Lógica del Caso de Uso)
    internal sealed record LoginUserQueryHandler : IRequestHandler<LoginUserQuery, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserQueryHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            // --- INICIO DE LÓGICA (Copiada de tu AuthUseCase) ---
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("Credenciales inválidas.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("Credenciales inválidas.");

            var userRoles = await _unitOfWork.UserRoleRepository.GetRolesByUserIdAsync(user.UserId);
            var roleNames = new List<string>();
            foreach (var userRole in userRoles)
            {
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(userRole.RoleId);
                if (role != null) roleNames.Add(role.RoleName);
            }

            string token = _jwtTokenGenerator.CreateToken(user, roleNames);

            return new AuthResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                Token = token
            };
        }
    }
}