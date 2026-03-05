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
        /// Maps a <see cref="Result{T}"/> to the appropriate <see cref="ActionResult"/>,
        /// returning a <see cref="ProblemDetails"/> response body on failure.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="result">The result to map.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><term>200 OK</term><description> with the value on success.</description></item>
        /// <item><term>400 Bad Request</term><description> when the result is <see cref="ResultType.Invalid"/>.</description></item>
        /// <item><term>404 Not Found</term><description> when the result is <see cref="ResultType.NotFound"/>.</description></item>
        /// <item><term>500 Internal Server Error</term><description> for any other failure.</description></item>
        /// </list>
        /// </returns>
        protected ActionResult OkOrError<T>(Result<T> result) => result.ResultType switch
        {
            ResultType.Ok => Ok(result.Value),
            ResultType.NotFound => NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.Message,
                Status = 404,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            }),
            ResultType.Invalid => BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.Message,
                Status = 400,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            }),
            _ => StatusCode(500, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = result.Message,
                Status = 500,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            })
        };
    }
}
