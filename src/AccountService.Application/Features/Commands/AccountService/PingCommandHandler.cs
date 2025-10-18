using AccountService.Common.Results;
using MediatR;

namespace AccountService.Application.Features.Commands.AccountService;

public class PingCommandHandler : IRequestHandler<PingCommand, Result<string>>
{

    public async Task<Result<string>> Handle(PingCommand pingCommand, CancellationToken cancellationToken)
    {
        List<string> tempDb = new();
        tempDb.Add(pingCommand.Message);
        return await Task.FromResult(Result<string>.Success(tempDb.First()));
    }
}
