using MediatR;
using AutoMapper;
using Lab11_AlberthMayta.Domain.Ports;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Domain.Ports;
using Lab10_AlberthMayta.Infrastructure;

namespace Lab11_AlberthMayta.Application.Features.Tickets.Commands.AddResponse
{
    // 1. El Comando (Petición)
    public record AddResponseCommand : IRequest<ResponseDto>
    {
        public Guid TicketId { get; set; }
        public string Message { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsCurrentUserAdmin { get; set; }
    }

    // 2. El Handler (Lógica)
    internal sealed record AddResponseCommandHandler : IRequestHandler<AddResponseCommand, ResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddResponseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto> Handle(AddResponseCommand request, CancellationToken cancellationToken)
        {
            // --- Lógica copiada de TicketUseCase ---
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket no encontrado");

            // Regla de Rol
            if (!request.IsCurrentUserAdmin && ticket.UserId != request.CurrentUserId)
            {
                throw new AccessViolationException("No tiene permiso para responder a este ticket.");
            }

            var responder = await _unitOfWork.UserRepository.GetByIdAsync(request.CurrentUserId);
            var newResponse = new Response
            {
                ResponseId = Guid.NewGuid(),
                TicketId = request.TicketId,
                ResponderId = request.CurrentUserId,
                Message = request.Message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ResponseRepository.AddAsync(newResponse);

            // Regla de Rol (Admin cambia estado)
            if (request.IsCurrentUserAdmin && ticket.Status == "abierto")
            {
                ticket.Status = "en_proceso";
                _unitOfWork.TicketRepository.Update(ticket);
            }

            await _unitOfWork.SaveAsync();

            // Mapeo (Manual para el nombre)
            var dto = _mapper.Map<ResponseDto>(newResponse);
            dto.ResponderUsername = responder?.Username ?? "Desconocido";
            return dto;
        }
    }
}