using AccountService.Api.Extensions;
using AccountService.Application.Features.Commands.AccountService;
using AccountService.Application.Features.Queries.AccountService;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDefaultInfo()
    {
        var result = await mediator.Send(new PingQuery());

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostDefaultInfo(string msg)
    {
        var result = await mediator.Send(new PingCommand(msg));
        return result.ToActionResult();
    }
}
