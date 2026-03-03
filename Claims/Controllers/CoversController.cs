using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Services.Cover.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoversController(ICoversService _coversService) : ControllerBase
    {
        [HttpPost("compute")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<GetCoverResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetCoverResponse>>> GetCoversAsync()
        {
            return Ok(await _coversService.GetCoversAsync());
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(GetCoverResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetCoverResponse?>> GetCoverAsync(string id)
        {
            GetCoverResponse? cover = await _coversService.GetCoverAsync(id);

            if (cover == null)
            {
                return NotFound();
            }

            return Ok(cover);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateCoverAsync(CreateCoverRequest request)
        {
            string? coverId = await _coversService.CreateCoverAsync(request);

            if (string.IsNullOrEmpty(coverId))
            {
                return BadRequest("Failed to create cover.");
            }

            return Created();
        }

        [HttpDelete("{id}/delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCoverAsync(string id)
        {
            bool isDeleted = await _coversService.DeleteCoverAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}