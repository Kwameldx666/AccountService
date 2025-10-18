using AccountService.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value!);
        }
        return new ObjectResult(result.ErrorMessage)
        {
            StatusCode = 500
        };
    }
}
