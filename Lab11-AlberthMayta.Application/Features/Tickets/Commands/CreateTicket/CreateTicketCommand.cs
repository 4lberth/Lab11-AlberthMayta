using MediatR;
using AutoMapper;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Domain.Ports;
using Lab10_AlberthMayta.Infrastructure;

namespace Lab10_AlberthMayta.Application.Features.Tickets.Commands.CreateTicket
{
    public record CreateTicketCommand : IRequest<TicketDetailDto>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; } // El Controller lo setea desde el Token
    }

    internal sealed record CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTicketCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TicketDetailDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            // --- LÓGICA (Copiada de tu TicketUseCase) ---
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);

            var newTicket = new Ticket
            {
                TicketId = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                UserId = request.UserId,
                Status = "abierto",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TicketRepository.AddAsync(newTicket);
            await _unitOfWork.SaveAsync();

            // Usamos AutoMapper
            var dto = _mapper.Map<TicketDetailDto>(newTicket);
            dto.CreatorUsername = user?.Username ?? "Desconocido";
            return dto;
        }
    }
}