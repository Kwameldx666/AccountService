using AccountService.Common.Results;
using MediatR;

namespace AccountService.Application.Features.Queries.AccountService;

public record PingQuery() : IRequest<Result<string>> { };