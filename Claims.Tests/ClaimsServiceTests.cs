using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Contracts.Validation;
using Claims.Domain;
using Claims.Infrastructure.Interfaces;
using Claims.Infrastructure.Result;
using Claims.Services;
using Claims.Services.Claim;
using Claims.Services.Claim.Interfaces;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Claims.Tests;

public class ClaimsServiceTests
{
    private readonly IClaimsContext _context = Substitute.For<IClaimsContext>();
    private readonly IClaimsService _claimsService;

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
        _claimsService = new ClaimsService(_context);
    }

    // ── CreateClaimAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateClaim_ValidRequest_ReturnsClaim()
    {
        CreateClaimRequest request = new(CoverId, Now, "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);
        _context.AddClaimAsync(Arg.Any<Claim>()).Returns(c => c.Arg<Claim>());

        Result<ClaimResponse> result = await _claimsService.CreateClaimAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(CoverId, result.Value.CoverId);
        Assert.Equal("Test claim", result.Value.Name);
        Assert.Equal(5000m, result.Value.DamageCost);
    }

    [Fact]
    public async Task CreateClaim_DamageCostExceedsLimit_RequestInvalid()
    {
        CreateClaimRequest request = new(CoverId, Now, "Test claim", ClaimType.BadWeather, 100001m);

        IList<ValidationResult> errors = RequestValidationHelper.Validate(request);

        Assert.Single(errors);
        Assert.Equal(ValidationErrors.CLAIM_DAMAGE_COST_EXCEEDS_LIMIT, errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateClaim_DamageCostAtLimit_ReturnsClaim()
    {
        CreateClaimRequest request = new(CoverId, Now, "Test claim", ClaimType.BadWeather, 100000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);
        _context.AddClaimAsync(Arg.Any<Claim>()).Returns(c => c.Arg<Claim>());

        Result<ClaimResponse> result = await _claimsService.CreateClaimAsync(request);

        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.NotNull(request);
    }

    [Fact]
    public async Task CreateClaim_CoverNotFound_ReturnsNotFound()
    {
        CreateClaimRequest request = new(CoverId, Now, "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns((Cover?)null);

        Result<ClaimResponse> result = await _claimsService.CreateClaimAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.ResultType);
        Assert.Equal(ResultCodes.COVER_NOT_FOUND, result.Message);
        await _context.DidNotReceive().AddClaimAsync(Arg.Any<Claim>());
    }

    [Fact]
    public async Task CreateClaim_CreatedBeforeCoverStartDate_ReturnsInvalid()
    {
        CreateClaimRequest request = new(CoverId, ValidCover.StartDate.AddDays(-1), "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);

        Result<ClaimResponse> result = await _claimsService.CreateClaimAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Invalid, result.ResultType);
        Assert.Equal(ResultCodes.CLAIM_CREATED_NOT_WITHIN_COVER_PERIOD, result.Message);
        await _context.DidNotReceive().AddClaimAsync(Arg.Any<Claim>());
    }

    [Fact]
    public async Task CreateClaim_CreatedAfterCoverEndDate_ReturnsInvalid()
    {
        CreateClaimRequest request = new(CoverId, ValidCover.EndDate.AddDays(1), "Test claim", ClaimType.BadWeather, 5000m);
        _context.GetCoverAsync(CoverId).Returns(ValidCover);

        Result<ClaimResponse> result = await _claimsService.CreateClaimAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Invalid, result.ResultType);
        Assert.Equal(ResultCodes.CLAIM_CREATED_NOT_WITHIN_COVER_PERIOD, result.Message);
        await _context.DidNotReceive().AddClaimAsync(Arg.Any<Claim>());
    }

    // ── GetClaimAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetClaim_ExistingId_ReturnsClaim()
    {
        Claim claim = new("claim-1", CoverId, Now, "Test", ClaimType.BadWeather, 5000m);
        _context.GetClaimAsync("claim-1").Returns(claim);

        Result<ClaimResponse> result = await _claimsService.GetClaimAsync("claim-1");

        Assert.NotNull(result.Value);
        Assert.Equal("claim-1", result.Value.Id);
    }

    [Fact]
    public async Task GetClaim_NonExistentId_ReturnsNotFound()
    {
        _context.GetClaimAsync("nonexistent").Returns((Claim?)null);

        Result<ClaimResponse> result = await _claimsService.GetClaimAsync("nonexistent");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.ResultType);
        Assert.Equal(ResultCodes.CLAIM_NOT_FOUND, result.Message);
    }

    // ── GetClaimsAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetClaims_ReturnsMappedClaims()
    {
        List<Claim> claims = [
            new("claim-1", CoverId, Now, "First",  ClaimType.BadWeather, 1000m),
            new("claim-2", CoverId, Now, "Second", ClaimType.Fire,       2000m),
        ];
        _context.GetClaimsAsync().Returns(claims);

        Result<IEnumerable<ClaimResponse>> result = await _claimsService.GetClaimsAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetClaims_NoClaims_ReturnsEmptyList()
    {
        _context.GetClaimsAsync().Returns([]);

        Result<IEnumerable<ClaimResponse>> result = await _claimsService.GetClaimsAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    // ── DeleteClaimAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteClaim_ExistingId_ReturnsTrue()
    {
        _context.DeleteClaimAsync("claim-1").Returns(true);

        Result<bool> result = await _claimsService.DeleteClaimAsync("claim-1");

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeleteClaim_NonExistentId_ReturnsNotFound()
    {
        _context.DeleteClaimAsync("nonexistent").Returns(false);

        Result<bool> result = await _claimsService.DeleteClaimAsync("nonexistent");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.ResultType);
        Assert.Equal(ResultCodes.CLAIM_NOT_FOUND, result.Message);
    }
}