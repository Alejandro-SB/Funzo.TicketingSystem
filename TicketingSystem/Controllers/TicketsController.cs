using Microsoft.AspNetCore.Mvc;
using TicketingSystem.UseCases.Tickets;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly CreateTicket _createTicket;
        private readonly GetTicket _getTicket;
        private readonly EscalateTicket _escalateTicket;
        private readonly AddCommentToTicket _addComment;
        private readonly SolveTicket _solveTicket;
        private readonly GetAllTickets _getAllTickets;

        public TicketsController(CreateTicket createTicket, GetTicket getTicket, EscalateTicket escalateTicket, AddCommentToTicket addComment, SolveTicket solveTicket, GetAllTickets getAllTickets)
        {
            _createTicket = createTicket;
            _getTicket = getTicket;
            _escalateTicket = escalateTicket;
            _addComment = addComment;
            _solveTicket = solveTicket;
            _getAllTickets = getAllTickets;
        }

        [HttpGet]
        public async Task<IResult> GetAll(CancellationToken cancellationToken)
        {
            var request = new GetAllTicketsRequest();

            var response = await _getAllTickets.Handle(request, cancellationToken);

            return Results.Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IResult> Get(int id, CancellationToken cancellationToken)
        {
            var request = new GetTicketRequest(id);

            var response = await _getTicket.Handle(request, cancellationToken);

            return Results.Ok(response);
        }

        [HttpPost]
        public async Task<IResult> Post([FromBody] CreateTicketHttpRequest httpRequest, CancellationToken cancellationToken)
        {
            var request = httpRequest.ToRequest();

            var response = await _createTicket.Handle(request, cancellationToken);

            return Results.Ok(response);
        }

        [HttpPost("{id:int}/escalate")]
        public async Task<IResult> EscalateTicket(int id, CancellationToken cancellationToken)
        {
            var request = new EscalateTicketRequest(id);

            var response = await _escalateTicket.Handle(request, cancellationToken);

            return Results.Ok(response);
        }

        [HttpPost("{id:int}/comments")]
        public async Task<IResult> AddComment(int id, [FromBody] AddCommentHttpRequest httpRequest, CancellationToken cancellationToken)
        {
            var request = httpRequest.ToRequest(id);

            var response = await _addComment.Handle(request, cancellationToken);

            return Results.Ok(response);
        }

        [HttpPost("{id:int}/solve")]
        public async Task<IResult> Solve(int id, CancellationToken cancellationToken)
        {
            var request = new SolveTicketRequest(id);

            var response = await _solveTicket.Handle(request, cancellationToken);

            return Results.Ok(response);
        }
    }

    public class CreateTicketHttpRequest
    {
        public int UserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public CreateTicketRequest ToRequest() => new(UserId, Subject, Body);
    }

    public class AddCommentHttpRequest
    {
        public int UserId { get; set; }
        public string Text { get; set; }

        public AddCommentToTicketRequest ToRequest(int ticketId) => new(ticketId, Text, UserId);
    }
}
