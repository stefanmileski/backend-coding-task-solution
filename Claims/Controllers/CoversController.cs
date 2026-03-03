using Claims.Domain;
using Microsoft.AspNetCore.Mvc;
using Claims.Services.Cover.Interfaces;

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
        public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
        {
            return Ok(await _coversService.GetCoversAsync());
        }

        [HttpGet("{uid}")]
        public async Task<ActionResult<Cover?>> GetAsync(Guid uid)
        {
            return Ok(await _coversService.GetCoverAsync(uid));
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Cover cover)
        {
            await _coversService.CreateCoverAsync(cover);
            return Ok(cover);
        }

        [HttpDelete("{uid}")]
        public async Task DeleteAsync(Guid uid)
        {
            await _coversService.DeleteCoverAsync(uid);
        }
    }
}