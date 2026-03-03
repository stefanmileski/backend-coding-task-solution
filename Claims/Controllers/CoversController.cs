using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Services.Cover.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoversController(ICoversService _coversService) : ControllerBase
    {
        [HttpPost("compute")]
        public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCoverResponse>>> GetAsync()
        {
            return Ok(await _coversService.GetCoversAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCoverResponse?>> GetAsync(string id)
        {
            GetCoverResponse? cover = await _coversService.GetCoverAsync(id);

            if (cover != null)
            {
                return Ok(cover);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateCoverRequest request)
        {
            string coverId = await _coversService.CreateCoverAsync(request);

            if (string.IsNullOrEmpty(coverId))
            {
                return BadRequest("Failed to create cover.");
            }

            return Ok(coverId);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            bool isDeleted = await _coversService.DeleteCoverAsync(id);

            if (isDeleted)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}