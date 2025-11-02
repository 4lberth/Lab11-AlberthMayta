using MediatR;
using AutoMapper;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Domain.Ports;
using Lab10_AlberthMayta.Infrastructure;

namespace Lab10_AlberthMayta.Application.Features.Users.Commands.RegisterUser
{
    // 1. El Comando (DTO de Petición)
    public record RegisterUserCommand : IRequest<UserDto>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // 2. El Handler (La Lógica del Caso de Uso)
    internal sealed record RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // --- INICIO DE LÓGICA (Copiada de tu AuthUseCase) ---
            var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("El email ya está registrado.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            var defaultRole = await _unitOfWork.RoleRepository.GetRoleByNameAsync("User");
            if (defaultRole == null)
                throw new Exception("El rol 'User' por defecto no existe.");

            var newUserRole = new UserRole
            {
                UserId = newUser.UserId,
                RoleId = defaultRole.RoleId,
                AssignedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.UserRoleRepository.AddAsync(newUserRole);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<UserDto>(newUser);
            // --- FIN DE LÓGICA ---
        }
    }
}