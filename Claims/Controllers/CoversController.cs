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
        [ProducesResponseType(typeof(IEnumerable<CoverResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CoverResponse>>> GetCoversAsync()
        {
            return Ok(await _coversService.GetCoversAsync());
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(CoverResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CoverResponse?>> GetCoverAsync(string id)
        {
            CoverResponse? cover = await _coversService.GetCoverAsync(id);

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
            CoverResponse? cover = await _coversService.CreateCoverAsync(request);

            if (cover is null)
            {
                return BadRequest("Failed to create cover.");
            }

            return Ok(cover);
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