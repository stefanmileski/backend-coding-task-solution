using Claims.Infrastructure.Result;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers.Base
{
    public class ApiControllerBase : ControllerBase
    {
        protected ActionResult OkOrError<T>(Result<T> result) => result.ResultType switch
        {
            ResultType.Ok => Ok(result.Value),
            ResultType.NotFound => NotFound(new ProblemDetails { Detail = result.Message, Status = 404 }),
            ResultType.Invalid => BadRequest(new ProblemDetails { Detail = result.Message, Status = 400 }),
            _ => StatusCode(500, new ProblemDetails { Detail = result.Message, Status = 500 })
        };
    }
}
