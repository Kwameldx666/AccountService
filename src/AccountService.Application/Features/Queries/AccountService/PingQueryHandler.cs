using AccountService.Common.Results;
using MediatR;

namespace AccountService.Application.Features.Queries.AccountService;

public class PingQueryHandler : IRequestHandler<PingQuery, Result<string>>
{
    public async Task<Result<string>> Handle(PingQuery pingQuery, CancellationToken cancellationToken)
        => await Task.FromResult(Result<string>.Success("Pong"));

}
