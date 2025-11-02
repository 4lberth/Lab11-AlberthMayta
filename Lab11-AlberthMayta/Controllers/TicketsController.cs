using MediatR;
using Lab11_AlberthMayta.Application.Features.Tickets.Queries.GetTicketById;
using Lab11_AlberthMayta.Application.Features.Tickets.Commands.AddResponse;
using Lab11_AlberthMayta.Application.Features.Tickets.Queries.GetAllTicketsForAdmin;
using Lab11_AlberthMayta.Application.Features.Tickets.Queries.GetMyTickets;
using Lab11_AlberthMayta.Application.Features.Tickets.Commands.UpdateTicketStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Application.Features.Tickets.Commands.CreateTicket;

namespace Lab11_AlberthMayta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos los endpoints requieren login
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // --- Helpers para leer el Token ---
        private Guid GetUserIdFromToken() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        private bool IsUserAdmin() => User.IsInRole("Admin");

        // --- Endpoints para User ---

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateTicket(CreateTicketRequest request)
        {
            var command = new CreateTicketCommand
            {
                Title = request.Title,
                Description = request.Description,
                UserId = GetUserIdFromToken()
            };
            var ticketDto = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTicketById), new { id = ticketDto.TicketId }, ticketDto);
        }

        [HttpGet("my-tickets")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyTickets()
        {
            var query = new GetMyTicketsQuery { CurrentUserId = GetUserIdFromToken() };
            var tickets = await _mediator.Send(query);
            return Ok(tickets);
        }

        // --- Endpoints para Admin ---

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTicketsForAdmin()
        {
            var query = new GetAllTicketsForAdminQuery();
            var tickets = await _mediator.Send(query);
            return Ok(tickets);
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateTicketStatusRequest request)
        {
            var command = new UpdateTicketStatusCommand 
            { 
                TicketId = id, 
                Status = request.Status 
            };
            await _mediator.Send(command);
            return NoContent();
        }

        // --- Endpoints Comunes (User y Admin) ---

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetTicketById(Guid id)
        {
            var query = new GetTicketByIdQuery 
            { 
                TicketId = id, 
                CurrentUserId = GetUserIdFromToken(), 
                IsCurrentUserAdmin = IsUserAdmin() 
            };

            try
            {
                var ticketDto = await _mediator.Send(query);
                return Ok(ticketDto);
            }
            catch (AccessViolationException ex) { return StatusCode(StatusCodes.Status403Forbidden, ex.Message); }
            catch (Exception ex) { return NotFound(ex.Message); }
        }
        
        [HttpPost("{id:guid}/responses")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddResponse(Guid id, AddResponseRequest request)
        {
            var command = new AddResponseCommand
            {
                TicketId = id,
                Message = request.Message,
                CurrentUserId = GetUserIdFromToken(),
                IsCurrentUserAdmin = IsUserAdmin()
            };

            try
            {
                var responseDto = await _mediator.Send(command);
                return Ok(responseDto);
            }
            catch (AccessViolationException ex) { return StatusCode(StatusCodes.Status403Forbidden, ex.Message); }
            catch (Exception ex) { return NotFound(ex.Message); }
        }
    }
}