using AccountService.Common.Results;
using MediatR;

namespace AccountService.Application.Features.Commands.AccountService;

public record PingCommand(string Message) : IRequest<Result<string>> { };
