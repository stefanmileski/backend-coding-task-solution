using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Controllers.Base;
using Claims.Domain;
using Claims.Services.Cover.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoversController(ICoversService _coversService) : ApiControllerBase
    {
        [HttpPost("compute")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<CoverResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CoverResponse>>> GetCoversAsync()
        {
            return OkOrError(await _coversService.GetCoversAsync());
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(CoverResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CoverResponse?>> GetCoverAsync(string id)
        {
            return OkOrError(await _coversService.GetCoverAsync(id));
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateCoverAsync(CreateCoverRequest request)
        {
            return OkOrError(await _coversService.CreateCoverAsync(request));
        }

        [HttpDelete("{id}/delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCoverAsync(string id)
        {
            return OkOrError(await _coversService.DeleteCoverAsync(id));
        }
    }
}