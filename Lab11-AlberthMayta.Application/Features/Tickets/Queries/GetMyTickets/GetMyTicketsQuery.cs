using MediatR;
using AutoMapper;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Domain.Ports;
using Lab11_AlberthMayta.Domain.Ports;

namespace Lab11_AlberthMayta.Application.Features.Tickets.Queries.GetMyTickets
{
    // 1. La Consulta (Petición)
    public record GetMyTicketsQuery : IRequest<IEnumerable<TicketSummaryDto>>
    {
        public Guid CurrentUserId { get; set; }
    }

    // 2. El Handler (Lógica)
    internal sealed record GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, IEnumerable<TicketSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMyTicketsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketSummaryDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            // --- Lógica copiada de TicketUseCase ---
            var tickets = await _unitOfWork.TicketRepository.GetTicketsByUserIdAsync(request.CurrentUserId);
            return _mapper.Map<IEnumerable<TicketSummaryDto>>(tickets);
        }
    }
}