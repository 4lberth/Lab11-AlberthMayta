using Lab10_AlberthMayta.Domain.Ports;
using MediatR;
using Lab11_AlberthMayta.Domain.Ports;

namespace Lab11_AlberthMayta.Application.Features.Tickets.Commands.UpdateTicketStatus
{
    // 1. El Comando (Petición)
    public record UpdateTicketStatusCommand : IRequest
    {
        public Guid TicketId { get; set; }
        public string Status { get; set; }
    }

    // 2. El Handler (Lógica)
    internal sealed record UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTicketStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
        {
            // --- Lógica copiada de TicketUseCase ---
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket no encontrado");

            ticket.Status = request.Status;
            if (request.Status == "cerrado")
            {
                ticket.ClosedAt = DateTime.UtcNow;
            }
            else
            {
                ticket.ClosedAt = null;
            }
            
            _unitOfWork.TicketRepository.Update(ticket);
            await _unitOfWork.SaveAsync();
        }
    }
}