using Claims.Core.Result;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers.Base
{
    /// <summary>
    /// Base controller providing shared response mapping for all API controllers.
    /// </summary>
    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Maps a <see cref="Result{T}"/> to the appropriate <see cref="ActionResult"/>.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="result">The result to map.</param>
        /// <returns>
        /// 200 OK with the value on success;
        /// 404 Not Found, 400 Bad Request, or 500 Internal Server Error with a <see cref="ProblemDetails"/> body on failure.
        /// </returns>
        protected ActionResult OkOrError<T>(Result<T> result) => result.ResultType switch
        {
            ResultType.Ok => Ok(result.Value),
            ResultType.NotFound => NotFound(new ProblemDetails { Detail = result.Message, Status = 404 }),
            ResultType.Invalid => BadRequest(new ProblemDetails { Detail = result.Message, Status = 400 }),
            _ => StatusCode(500, new ProblemDetails { Detail = result.Message, Status = 500 })
        };
    }
}
