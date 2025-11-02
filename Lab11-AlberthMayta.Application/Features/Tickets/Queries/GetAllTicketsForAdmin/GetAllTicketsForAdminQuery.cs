using MediatR;
using AutoMapper;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Domain.Ports;
using Lab11_AlberthMayta.Domain.Ports;

namespace Lab11_AlberthMayta.Application.Features.Tickets.Queries.GetAllTicketsForAdmin
{
    // 1. La Consulta (Petición)
    public record GetAllTicketsForAdminQuery : IRequest<IEnumerable<TicketSummaryDto>>
    {
        // No necesita parámetros
    }

    // 2. El Handler (Lógica)
    internal sealed record GetAllTicketsForAdminQueryHandler : IRequestHandler<GetAllTicketsForAdminQuery, IEnumerable<TicketSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllTicketsForAdminQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketSummaryDto>> Handle(GetAllTicketsForAdminQuery request, CancellationToken cancellationToken)
        {
            // --- Lógica copiada de TicketUseCase ---
            var tickets = await _unitOfWork.TicketRepository.GetOpenTicketsWithUserAsync();
            
            // AutoMapper se encarga de mapear el CreatorUsername
            return _mapper.Map<IEnumerable<TicketSummaryDto>>(tickets);
        }
    }
}