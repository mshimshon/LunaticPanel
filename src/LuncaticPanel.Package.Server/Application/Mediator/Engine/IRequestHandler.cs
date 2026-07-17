namespace LuncaticPanel.Package.Server.Application.Mediator.Engine;


public interface IRequestHandler<TCommand> : IRequest
{
    Task HandleAsync(TCommand data, CancellationToken ct = default);
}

public interface IRequestHandler<TCommand, TResult> where TCommand : IRequest
{
    Task<TResult> HandleAsync(TCommand data, CancellationToken ct = default);
}
