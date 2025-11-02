using MediatR;
using AutoMapper;
using Lab11_AlberthMayta.Domain.Ports;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Domain.Ports;

namespace Lab11_AlberthMayta.Application.Features.Tickets.Queries.GetTicketById
{
    // 1. La Consulta (DTO de Petición)
    // Le pasamos los datos de seguridad (UserId, IsAdmin) desde el Controller
    public record GetTicketByIdQuery : IRequest<TicketDetailDto>
    {
        public Guid TicketId { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsCurrentUserAdmin { get; set; }
    }

    // 2. El Handler (La Lógica del Caso de Uso)
    internal sealed record GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTicketByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TicketDetailDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            // --- INICIO DE LÓGICA (Copiada de tu TicketUseCase) ---
            var ticket = await _unitOfWork.TicketRepository.GetTicketWithUserByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket no encontrado"); // El Controller lo volverá 404

            // --- REGLA DE ROL (La lógica de seguridad clave) ---
            if (!request.IsCurrentUserAdmin && ticket.UserId != request.CurrentUserId)
            {
                throw new AccessViolationException("No tiene permiso para ver este ticket."); // El Controller lo volverá 403
            }

            // Cargar respuestas
            var responses = await _unitOfWork.ResponseRepository.GetResponsesByTicketIdAsync(request.TicketId);
            var responderIds = responses.Select(r => r.ResponderId).Distinct();
            var responders = (await _unitOfWork.UserRepository.GetAllAsync())
                                .Where(u => responderIds.Contains(u.UserId));

            // Mapear usando AutoMapper
            var ticketDto = _mapper.Map<TicketDetailDto>(ticket);
            
            // Mapear manualmente las respuestas (porque AutoMapper necesita ayuda con el 'responder')
            ticketDto.Responses = responses.Select(r => new ResponseDto
            {
                ResponseId = r.ResponseId,
                ResponderId = r.ResponderId,
                Message = r.Message,
                CreatedAt = r.CreatedAt,
                ResponderUsername = responders.FirstOrDefault(u => u.UserId == r.ResponderId)?.Username ?? "Desconocido"
            }).ToList();
            
            return ticketDto;
            // --- FIN DE LÓGICA ---
        }
    }
}