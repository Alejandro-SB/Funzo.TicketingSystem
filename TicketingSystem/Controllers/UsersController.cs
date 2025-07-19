using Microsoft.AspNetCore.Mvc;
using TicketingSystem.UseCases.Users;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly CreateUser _createUser;
        private readonly GetUser _getUser;
        private readonly GetUserComments _getUserComments;

        public UsersController(CreateUser createUser, GetUser getUser, GetUserComments getUserComments)
        {
            _createUser = createUser;
            _getUser = getUser;
            _getUserComments = getUserComments;
        }

        [HttpGet("{id:int}")]
        public async Task<IResult> Get(int id, CancellationToken cancellationToken)
        {
            var request = new GetUserRequest(id);

            var response = await _getUser.Handle(request, cancellationToken);

            return Results.Ok(response);
        }

        [HttpPost]
        public async Task<IResult> Post([FromBody] CreateUserHttp httpRequest, CancellationToken cancellationToken)
        {
            var request = httpRequest.ToRequest();

            var response = await _createUser.Handle(request, cancellationToken);

            return Results.Ok(response);
        }

        [HttpGet("{id:int}/comments")]
        public async Task<IResult> GetAllComments(int id, CancellationToken cancellationToken)
        {
            var request = new GetUserCommentsRequest(id);

            var response = await _getUserComments.Handle(request, cancellationToken);

            return Results.Ok(response);
        }
    }

    public class CreateUserHttp
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }

        public CreateUserRequest ToRequest() => new(Username, DisplayName);
    }
}
