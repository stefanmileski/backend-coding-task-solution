using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Infrastructure.Interfaces;
using Claims.Services.Claim;
using Claims.Services.Claim.Interfaces;
using NSubstitute;
using Xunit;

namespace Claims.Tests;

public class ClaimsServiceTests
{
    private readonly IClaimsContext _context = Substitute.For<IClaimsContext>();
    private readonly IClaimsService _sut;

    // Reusable test data
    private static readonly string CoverId = Guid.NewGuid().ToString();
    private static readonly DateTime Now = DateTime.UtcNow;
    private static readonly Cover ValidCover = new(
        id: CoverId,
        startDate: Now.AddDays(-10),
        endDate: Now.AddDays(10),
        type: CoverType.Yacht,
        premium: 1000m);

    public ClaimsServiceTests()
    {
        _sut = new ClaimsService(_context);
    }

    // ── CreateClaimAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateClaim_ValidRequest_ReturnsClaim()
    {
        var request = new CreateClaimRequest(CoverId, Now, "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);
        _context.AddClaimAsync(Arg.Any<Claim>()).Returns(c => c.Arg<Claim>());

        ClaimResponse? result = await _sut.CreateClaimAsync(request);

        Assert.NotNull(result);
        Assert.Equal(CoverId, result.CoverId);
        Assert.Equal("Test claim", result.Name);
        Assert.Equal(5000m, result.DamageCost);
    }

    [Fact]
    public async Task CreateClaim_DamageCostExceedsLimit_ReturnsNull()
    {
        var request = new CreateClaimRequest(CoverId, Now, "Test claim", ClaimType.BadWeather, 100001m);

        ClaimResponse? result = await _sut.CreateClaimAsync(request);

        Assert.Null(result);
        await _context.DidNotReceive().AddClaimAsync(Arg.Any<Claim>());
    }

    [Fact]
    public async Task CreateClaim_DamageCostAtLimit_ReturnsClaim()
    {
        var request = new CreateClaimRequest(CoverId, Now, "Test claim", ClaimType.BadWeather, 100000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);
        _context.AddClaimAsync(Arg.Any<Claim>()).Returns(c => c.Arg<Claim>());

        ClaimResponse? result = await _sut.CreateClaimAsync(request);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateClaim_CoverNotFound_ReturnsNull()
    {
        var request = new CreateClaimRequest(CoverId, Now, "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns((Cover?)null);

        ClaimResponse? result = await _sut.CreateClaimAsync(request);

        Assert.Null(result);
        await _context.DidNotReceive().AddClaimAsync(Arg.Any<Claim>());
    }

    [Fact]
    public async Task CreateClaim_CreatedBeforeCoverStartDate_ReturnsNull()
    {
        var request = new CreateClaimRequest(CoverId, ValidCover.StartDate.AddDays(-1), "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);

        ClaimResponse? result = await _sut.CreateClaimAsync(request);

        Assert.Null(result);
        await _context.DidNotReceive().AddClaimAsync(Arg.Any<Claim>());
    }

    [Fact]
    public async Task CreateClaim_CreatedAfterCoverEndDate_ReturnsNull()
    {
        var request = new CreateClaimRequest(CoverId, ValidCover.EndDate.AddDays(1), "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);

        ClaimResponse? result = await _sut.CreateClaimAsync(request);

        Assert.Null(result);
        await _context.DidNotReceive().AddClaimAsync(Arg.Any<Claim>());
    }

    // ── GetClaimAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetClaim_ExistingId_ReturnsClaim()
    {
        var claim = new Claim("claim-1", CoverId, Now, "Test", ClaimType.BadWeather, 5000m);
        _context.GetClaimAsync("claim-1").Returns(claim);

        ClaimResponse? result = await _sut.GetClaimAsync("claim-1");

        Assert.NotNull(result);
        Assert.Equal("claim-1", result.Id);
    }

    [Fact]
    public async Task GetClaim_NonExistentId_ReturnsNull()
    {
        _context.GetClaimAsync("nonexistent").Returns((Claim?)null);

        ClaimResponse? result = await _sut.GetClaimAsync("nonexistent");

        Assert.Null(result);
    }

    // ── GetClaimsAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetClaims_ReturnsMappedClaims()
    {
        var claims = new List<Claim>
        {
            new("claim-1", CoverId, Now, "First",  ClaimType.BadWeather, 1000m),
            new("claim-2", CoverId, Now, "Second", ClaimType.Fire,       2000m),
        };
        _context.GetClaimsAsync().Returns(claims);

        IEnumerable<ClaimResponse> result = await _sut.GetClaimsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetClaims_NoClaims_ReturnsEmptyList()
    {
        _context.GetClaimsAsync().Returns([]);

        IEnumerable<ClaimResponse> result = await _sut.GetClaimsAsync();

        Assert.Empty(result);
    }

    // ── DeleteClaimAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteClaim_ExistingId_ReturnsTrue()
    {
        _context.DeleteClaimAsync("claim-1").Returns(true);

        bool result = await _sut.DeleteClaimAsync("claim-1");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteClaim_NonExistentId_ReturnsFalse()
    {
        _context.DeleteClaimAsync("nonexistent").Returns(false);

        bool result = await _sut.DeleteClaimAsync("nonexistent");

        Assert.False(result);
    }
}