using Microsoft.AspNetCore.Mvc;

namespace EMerx.ResultPattern;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.Error.Code switch
        {
            404 => new NotFoundObjectResult(result.Error.Description),
            400 => new BadRequestObjectResult(result.Error.Description),
            401 => new UnauthorizedObjectResult(result.Error.Description),
            403 => new ForbidResult(),
            409 => new ConflictObjectResult(result.Error.Description),

            _ => new BadRequestObjectResult(result.Error)
        };
    }
    
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        return result.Error.Code switch
        {
            404 => new NotFoundObjectResult(result.Error.Description),
            400 => new BadRequestObjectResult(result.Error.Description),
            401 => new UnauthorizedObjectResult(result.Error.Description),
            403 => new ForbidResult(),
            409 => new ConflictObjectResult(result.Error.Description),

            _   => new BadRequestObjectResult(result.Error)
        };
    }
}