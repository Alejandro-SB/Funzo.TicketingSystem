namespace TicketingSystem.UseCases;

public interface IUseCase<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
