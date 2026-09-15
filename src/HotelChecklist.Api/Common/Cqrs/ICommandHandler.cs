using HotelChecklist.Domain.Common;

namespace HotelChecklist.Api.Common.Cqrs;

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
