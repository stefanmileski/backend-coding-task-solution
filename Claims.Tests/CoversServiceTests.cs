using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Contracts.Validation;
using Claims.Domain;
using Claims.Infrastructure.Interfaces;
using Claims.Infrastructure.Result;
using Claims.Services;
using Claims.Services.Cover;
using Claims.Services.Cover.Interfaces;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Claims.Tests;

public class CoversServiceTests
{
    private readonly IClaimsContext _context = Substitute.For<IClaimsContext>();
    private readonly ICoversService _coversService;

    private static readonly DateTime Today = DateTime.UtcNow.Date;

    public CoversServiceTests()
    {
        _coversService = new CoversService(_context);
    }

    // ── CreateCoverAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCover_ValidRequest_ReturnsCover()
    {
        CreateCoverRequest request = new(Today, Today.AddMonths(3), CoverType.Yacht);
        _context.AddCoverAsync(Arg.Any<Cover>()).Returns(c => c.Arg<Cover>());

        Result<CoverResponse> result = await _coversService.CreateCoverAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(CoverType.Yacht, result.Value.Type);
        Assert.Equal(Today, result.Value.StartDate);
        Assert.Equal(Today.AddMonths(3), result.Value.EndDate);
        Assert.True(result.Value.Premium > 0);
    }

    [Fact]
    public async Task CreateCover_StartDateInPast_ReturnsNull()
    {
        CreateCoverRequest request = new(Today.AddDays(-1), Today.AddMonths(3), CoverType.Yacht);

        IList<ValidationResult> errors = RequestValidationHelper.Validate(request);

        Assert.Single(errors);
        Assert.Equal(ValidationErrors.START_DATE_IN_PAST, errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateCover_PeriodExceedsOneYear_ReturnsNull()
    {
        CreateCoverRequest request = new(Today, Today.AddYears(1).AddDays(1), CoverType.Yacht);

        IList<ValidationResult> errors = RequestValidationHelper.Validate(request);

        Assert.Single(errors);
        Assert.Equal(ValidationErrors.END_DATE_TOO_FAR, errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateCover_PeriodExactlyOneYear_ReturnsCover()
    {
        CreateCoverRequest request = new(Today, Today.AddYears(1), CoverType.Yacht);
        _context.AddCoverAsync(Arg.Any<Cover>()).Returns(c => c.Arg<Cover>());

        Result<CoverResponse> result = await _coversService.CreateCoverAsync(request);

        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(CoverType.Yacht, result.Value.Type);
        Assert.Equal(Today, result.Value.StartDate);
        Assert.Equal(Today.AddYears(1), result.Value.EndDate);
        Assert.True(result.Value.Premium > 0);

    }

    [Fact]
    public async Task CreateCover_EndDateBeforeStartDate_ReturnsNull()
    {
        CreateCoverRequest request = new(Today, Today.AddDays(-1), CoverType.Yacht);

        IList<ValidationResult> errors = RequestValidationHelper.Validate(request);

        Assert.Single(errors);
        Assert.Equal(ValidationErrors.END_DATE_BEFORE_START_DATE, errors[0].ErrorMessage);
    }

    // ── GetCoverAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetCover_ExistingId_ReturnsCover()
    {
        Cover cover = new("cover-1", Today, Today.AddMonths(3), CoverType.Yacht, 1000m);
        _context.GetCoverAsync("cover-1").Returns(cover);

        Result<CoverResponse> result = await _coversService.GetCoverAsync("cover-1");

        Assert.NotNull(result.Value);
        Assert.Equal("cover-1", result.Value.Id);
    }

    [Fact]
    public async Task GetCover_NonExistentId_ReturnsNull()
    {
        _context.GetCoverAsync("nonexistent").Returns((Cover?)null);

        Result<CoverResponse> result = await _coversService.GetCoverAsync("nonexistent");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.ResultType);
        Assert.Equal(ResultCodes.COVER_NOT_FOUND, result.Message);
    }

    // ── GetCoversAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetCovers_ReturnsAllCovers()
    {
        List<Cover> covers = [
            new("cover-1", Today, Today.AddMonths(3), CoverType.Yacht,         1000m),
            new("cover-2", Today, Today.AddMonths(6), CoverType.PassengerShip, 2000m),
        ];

        _context.GetCoversAsync().Returns(covers);

        Result<IEnumerable<CoverResponse>> result = await _coversService.GetCoversAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetCovers_NoCovers_ReturnsEmptyList()
    {
        _context.GetCoversAsync().Returns([]);

        Result<IEnumerable<CoverResponse>> result = await _coversService.GetCoversAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    // ── DeleteCoverAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteCover_ExistingId_ReturnsTrue()
    {
        _context.DeleteCoverAsync("cover-1").Returns(true);

        Result<bool> result = await _coversService.DeleteCoverAsync("cover-1");

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeleteCover_NonExistentId_ReturnsFalse()
    {
        _context.DeleteCoverAsync("nonexistent").Returns(false);

        Result<bool> result = await _coversService.DeleteCoverAsync("nonexistent");

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.NotFound, result.ResultType);
        Assert.Equal(ResultCodes.COVER_NOT_FOUND, result.Message);
    }

    // ── ComputePremium ──────────────────────────────────────────────────────

    [Theory]
    [InlineData(CoverType.Yacht, 1, 1375.00)]   // 1250 * 1.1
    [InlineData(CoverType.PassengerShip, 1, 1500.00)]   // 1250 * 1.2
    [InlineData(CoverType.Tanker, 1, 1875.00)]   // 1250 * 1.5
    [InlineData(CoverType.BulkCarrier, 1, 1625.00)]   // 1250 * 1.3
    public void ComputePremium_SingleDay_ReturnsCorrectRate(CoverType type, int days, decimal expected)
    {
        Result<decimal> result = _coversService.ComputePremium(Today, Today.AddDays(days - 1), type);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public void ComputePremium_Yacht_30Days_NoDiscount()
    {
        decimal premiumPerDay = 1250m * 1.1m;
        decimal expected = premiumPerDay * 30;

        Result<decimal> result = _coversService.ComputePremium(Today, Today.AddDays(29), CoverType.Yacht);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public void ComputePremium_Yacht_31Days_AppliesFirstDiscount()
    {
        decimal premiumPerDay = 1250m * 1.1m;
        decimal expected = premiumPerDay * 30           // first 30 days
                         + premiumPerDay * 0.95m * 1;  // day 31 at 5% discount

        Result<decimal> result = _coversService.ComputePremium(Today, Today.AddDays(30), CoverType.Yacht);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public void ComputePremium_Yacht_181Days_AppliesSecondDiscount()
    {
        decimal premiumPerDay = 1250m * 1.1m;
        decimal expected = premiumPerDay * 30           // days 0–29
                         + premiumPerDay * 0.95m * 150 // days 30–179
                         + premiumPerDay * 0.92m * 1;  // day 180 at 8% discount

        Result<decimal> result = _coversService.ComputePremium(Today, Today.AddDays(180), CoverType.Yacht);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public void ComputePremium_NonYacht_181Days_AppliesSecondDiscount()
    {
        decimal premiumPerDay = 1250m * 1.3m;
        decimal expected = premiumPerDay * 30          // days 0–29
                         + premiumPerDay * 0.98m * 150 // days 30–179
                         + premiumPerDay * 0.97m * 1;  // day 180 at 3% discount

        Result<decimal> result = _coversService.ComputePremium(Today, Today.AddDays(180), CoverType.BulkCarrier);

        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public void ComputePremium_EndDateBeforeStartDate_ReturnsError()
    {
        Result<decimal> result = _coversService.ComputePremium(Today, Today.AddDays(-1), CoverType.Yacht);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultType.Invalid, result.ResultType);
        Assert.Equal(ResultCodes.END_DATE_BEFORE_START_DATE, result.Message);
    }
}